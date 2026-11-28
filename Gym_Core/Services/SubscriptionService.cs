using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;
using Gym_Core.Enums;
using Gym_Core.Exceptions;
using Gym_Core.Interfaces;
using Gym_Core.Models;
using Microsoft.Extensions.Logging;

namespace Gym_Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<SubscriptionService> logger;

        public SubscriptionService(IUnitOfWork unitOfWork, ILogger<SubscriptionService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        private static SubscriptionStatus GetEffectiveStatus(Subscription s)
        {
            if (s.Status != SubscriptionStatus.Active)
                return s.Status;

            var expiredByDate = s.EndDate < DateTime.UtcNow.Date;
            var expiredBySessions = s.Plan.SessionsCount > 0 && s.SessionsRemaining <= 0;

            return (expiredByDate || expiredBySessions)
                ? SubscriptionStatus.Expired
                : SubscriptionStatus.Active;
        }  // To Know the real status of sub

        private static SubscriptionResponseDto MapToResponse(Subscription subscription)
        {
            return new SubscriptionResponseDto
            {
                Id = subscription.Id,
                PlanId = subscription.PlanId,
                MemberId = subscription.MemberId,
                MemberName = subscription.Member.FullName,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                PaidAmount = subscription.PaidAmount,
                PlanName = subscription.Plan.Name,
                Status = GetEffectiveStatus(subscription),
                RefundAmount = subscription.RefundAmount,
                SessionsRemaining = subscription.SessionsRemaining,
                FreezeEndsAt = subscription.FreezeEndsAt,
                FrozenAt = subscription.FrozenAt,
                CreatedAt = subscription.CreatedAt,
                CreatorName = subscription.Creator != null
                    ? subscription.Creator.FirstName + " " + subscription.Creator.LastName
                    : "",
            };
        }
        public async Task<SubscriptionResponseDto> CreateAsync(CreateSubscriptionDto dto, int creatorId)
        {
            var ExistingMember = await unitOfWork.Members.GetAsync(m => m.Id == dto.MemberId);
            if (ExistingMember == null || ExistingMember.IsActive == false)
                throw new NotFoundException("This Member wasn't found!");

            var ExistingPlan = await unitOfWork.Plans.GetAsync(m => m.Id == dto.PlanId);
            if (ExistingPlan == null || ExistingPlan.IsActive == false)
                throw new NotFoundException("This Plan wasn't found!");

            var MemberSubscriptions = await unitOfWork.Subscriptions.
                GetAllAsync(s => (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Frozen)
                            && s.MemberId == ExistingMember.Id, new[] { "Plan" });

            foreach (var sub in MemberSubscriptions)
            {
                if (sub.Status == SubscriptionStatus.Frozen ||
                    GetEffectiveStatus(sub) == SubscriptionStatus.Active)
                {
                    sub.Status = SubscriptionStatus.Cancelled;
                    sub.CancelledAt = DateTime.UtcNow;
                    sub.CancelReason = "Replaced by renewal";
                    sub.FrozenAt = null;
                    sub.FreezeEndsAt = null;
                }
            }

            var today = DateTime.UtcNow.Date;
            var Subscription = new Subscription
            {
                PlanId = dto.PlanId,
                MemberId = dto.MemberId,
                CreatedAt = DateTime.UtcNow,
                CreatorId = creatorId,
                PaidAmount = ExistingPlan.Price,
                StartDate = today,
                EndDate = today.AddDays(ExistingPlan.DurationInDays!.Value - 1),
                RefundAmount = 0,
                SessionsRemaining = ExistingPlan.SessionsCount,
                Status = SubscriptionStatus.Active,
                Member = ExistingMember,
                Plan = ExistingPlan,
                TotalFrozenDays = 0
            };

            await unitOfWork.Subscriptions.AddAsync(Subscription);
            await unitOfWork.CompleteAsync();

            logger.LogInformation("Subscription created for MemberId {MemberId} on PlanId {PlanId} by CreatorId {CreatorId}",
                dto.MemberId, dto.PlanId, creatorId);

            return MapToResponse(Subscription);
        }

        public async Task<SubscriptionResponseDto> CancelAsync(int id, CancelSubscriptionDto dto)
        {
            var Existing = await unitOfWork.Subscriptions.GetAsync(s => s.Id == id, new[] { "Member", "Plan" });

            if (Existing is null)
                throw new NotFoundException("This subscription was not found");

            if (Existing.Status != SubscriptionStatus.Active && Existing.Status != SubscriptionStatus.Frozen)
                throw new BadRequestException("Only an active subscriptions can be cancelled");

            if (dto.RefundAmount < 0 || dto.RefundAmount > Existing.PaidAmount)
                throw new BadRequestException("Refund amount must be between 0 and the paid amount.");

            Existing.Status = SubscriptionStatus.Cancelled;
            Existing.CancelledAt = DateTime.UtcNow;
            Existing.CancelReason = dto.Reason.Trim();
            Existing.RefundAmount = dto.RefundAmount;
            Existing.FrozenAt = null;
            Existing.FreezeEndsAt = null;

            await unitOfWork.CompleteAsync();

            logger.LogInformation("Subscription {SubscriptionId} cancelled with refund {RefundAmount}",
                id, dto.RefundAmount);

            return MapToResponse(Existing);
        }

        public async Task<PagedResultDTO<SubscriptionResponseDto>> GetAllSubscriptionsAsync(int page, bool IncludingExpired = false)
        {
            await UnfreezeExpiredSubscriptionsAsync();  // for subscriptions frozen time over

            var today = DateTime.UtcNow.Date;

            var (subs, total) = await unitOfWork.Subscriptions.GetPagedAsync(page, s => IncludingExpired
                            || (s.EndDate >= today
                            && (s.Plan.SessionsCount == 0 || s.SessionsRemaining > 0)), new[] { "Member", "Plan", "Creator" });


            return new PagedResultDTO<SubscriptionResponseDto>
            {
                Items = subs.Select(MapToResponse).ToList(),
                Page = page,
                PageSize = 20,
                TotalCount = total
            };
        }

        public async Task<SubscriptionResponseDto> GetByIdAsync(int id)
        {
            var Existing = await unitOfWork.Subscriptions.GetAsync(s => s.Id == id, new[] { "Member", "Plan" });

            if (Existing is null)
                throw new NotFoundException("This subscription was not found");

            return MapToResponse(Existing);
        }

        public async Task<IEnumerable<SubscriptionResponseDto>> GetMemberSubscriptionsAsync(int memberId)
        {
            var member = await unitOfWork.Members.GetAsync(m => m.Id == memberId);

            if (member == null)
                throw new NotFoundException($"This member is not found");

            var subs = await unitOfWork.Subscriptions.GetAllAsync(s => s.MemberId == memberId, new[] { "Member", "Plan" });

            return subs
                .OrderByDescending(s => s.StartDate)
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<SubscriptionResponseDto> FreezeAsync(int Id, FreezeSubscriptionDTO dto)
        {
            var subscription = await unitOfWork.Subscriptions.GetAsync(s => s.Id == Id, new[] { "Member", "Plan" });

            if (subscription == null)
                throw new NotFoundException("This subscription was not found");

            if (GetEffectiveStatus(subscription) != SubscriptionStatus.Active)
                throw new BadRequestException("Only an active subscription can be frozen");

            if (subscription.TotalFrozenDays > 0)
                throw new BadRequestException("This subscription was already frozen once");

            if (subscription.EndDate.Date <= DateTime.UtcNow.Date)
                throw new BadRequestException("Subscription is about to end, cannot freeze");

            subscription.Status = SubscriptionStatus.Frozen;
            subscription.FrozenAt = DateTime.UtcNow.Date;
            subscription.FreezeReason = dto.Reason?.Trim();
            subscription.FreezeEndsAt = DateTime.UtcNow.Date.AddDays(dto.Days);

            await unitOfWork.CompleteAsync();

            logger.LogInformation("Subscription {SubscriptionId} frozen for {Days} days", Id, dto.Days);

            return MapToResponse(subscription);
        }

        public async Task<SubscriptionResponseDto> UnFreezeAsync(int Id)
        {
            var subscription = await unitOfWork.Subscriptions.GetAsync(s => s.Id == Id, new[] { "Member", "Plan" });

            if (subscription is null)
                throw new NotFoundException("This subscription was not found");

            if (subscription.Status != SubscriptionStatus.Frozen)
                throw new BadRequestException($"this subscription is {subscription.Status}");

            var today = DateTime.UtcNow.Date;
            var frozenAt = subscription.FrozenAt!.Value.Date;
            var plannedEnd = subscription.FreezeEndsAt!.Value.Date;

            var effectiveEnd = today < plannedEnd ? today : plannedEnd;     // if late unfreeze -> plannedEnd will be actual end 
            var actualDays = (effectiveEnd - frozenAt).Days;

            subscription.EndDate = subscription.EndDate.AddDays(actualDays);

            subscription.Status = SubscriptionStatus.Active;
            subscription.TotalFrozenDays += actualDays;
            subscription.FrozenAt = null;
            subscription.FreezeEndsAt = null;

            await unitOfWork.CompleteAsync();

            logger.LogInformation("Subscription {SubscriptionId} unfrozen after {ActualDays} days", Id, actualDays);

            return MapToResponse(subscription);
        }

        public async Task UnfreezeExpiredSubscriptionsAsync()
        {
            var today = DateTime.UtcNow.Date;

            var expiredFrozenList = await unitOfWork.Subscriptions.GetAllAsync(
                s => s.Status == SubscriptionStatus.Frozen &&
                     today >= s.FreezeEndsAt!.Value.Date);

            var subscriptions = expiredFrozenList.ToList();
            if (subscriptions.Count == 0)
                return;

            foreach (var sub in subscriptions)
            {
                var frozenDays = (sub.FreezeEndsAt!.Value.Date - sub.FrozenAt!.Value.Date).Days;

                sub.TotalFrozenDays += frozenDays;
                sub.EndDate = sub.EndDate.AddDays(frozenDays);
                sub.Status = sub.EndDate.Date < today
                    ? SubscriptionStatus.Expired
                    : SubscriptionStatus.Active;

                sub.FrozenAt = null;
                sub.FreezeEndsAt = null;
            }

            await unitOfWork.CompleteAsync();

            logger.LogInformation("Auto-unfroze {Count} expired frozen subscriptions", subscriptions.Count);
        }

        public async Task<RevenueDTO> GetRevenueAsync(DateTime from, DateTime to)
        {
            var start = from.Date;
            var end = to.Date.AddDays(1);

            var created = await unitOfWork.Subscriptions.GetAllAsync(
                s => s.CreatedAt >= start && s.CreatedAt < end);

            var cancelled = await unitOfWork.Subscriptions.GetAllAsync(
                s => s.CancelledAt != null && s.CancelledAt >= start && s.CancelledAt < end);

            return new RevenueDTO
            {
                TotalPaid = created.Sum(s => s.PaidAmount)!.Value,
                TotalRefunded = cancelled.Sum(s => s.RefundAmount),
                SubscriptionsCount = created.Count()
            };
        }
    }
}