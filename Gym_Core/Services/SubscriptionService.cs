using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;
using Gym_Core.Enums;
using Gym_Core.Exceptions;
using Gym_Core.Interfaces;
using Gym_Core.Models;

namespace Gym_Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork unitOfWork;

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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
                SessionsRemaining = subscription.SessionsRemaining
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

            var ActiveMemberSubscription = await unitOfWork.Subscriptions.
                GetAllAsync(s => s.Status == SubscriptionStatus.Active && s.MemberId == ExistingMember.Id, new[] { "Plan" });
            foreach (var sub in ActiveMemberSubscription.Where(sub => GetEffectiveStatus(sub) == SubscriptionStatus.Active))
            {
                sub.Status = SubscriptionStatus.Cancelled;
                sub.CancelledAt = DateTime.UtcNow;
                sub.CancelReason = "Reblaced by renewal!";
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
                Plan = ExistingPlan
            };

            await unitOfWork.Subscriptions.AddAsync(Subscription);
            await unitOfWork.CompleteAsync();

            return MapToResponse(Subscription);
        }

        public async Task<SubscriptionResponseDto> CancelAsync(int id, CancelSubscriptionDto dto)
        {
            var Existing = await unitOfWork.Subscriptions.GetAsync(s => s.Id == id, new[] { "Member", "Plan" });

            if (Existing is null)
                throw new NotFoundException("This subscription was not found");

            if (GetEffectiveStatus(Existing) != SubscriptionStatus.Active)
                throw new BadRequestException("Only an active subscriptions can be cancelled");

            if (dto.RefundAmount < 0 || dto.RefundAmount > Existing.PaidAmount)
                throw new BadRequestException("Refund amount must be between 0 and the paid amount.");

            Existing.Status = SubscriptionStatus.Cancelled; 
            Existing.CancelledAt = DateTime.UtcNow.Date;
            Existing.CancelReason = dto.Reason.Trim();
            Existing.RefundAmount = dto.RefundAmount;    

            await unitOfWork.CompleteAsync();

            return MapToResponse(Existing);
        }

        public async Task<IEnumerable<SubscriptionResponseDto>> GetAllSubscriptionsAsync(bool IncludingExpired = false)
        {
            var subs = await unitOfWork.Subscriptions.GetAllAsync(null!, new[] { "Member", "Plan" });

            var res = subs; 
            if(!IncludingExpired)
                 res = subs.Where(s => s.Status != SubscriptionStatus.Expired);

            return res.Select(MapToResponse);
        }

        public async Task<SubscriptionResponseDto> GetByIdAsync(int id)
        {
            var Existing = await unitOfWork.Subscriptions.GetAsync(s => s.Id == id, new[] { "Member", "Plan"});

            if (Existing is null)
                throw new NotFoundException("This subscription was not found");

            return MapToResponse(Existing);
        }

        public async Task<IEnumerable<SubscriptionResponseDto>> GetMemberSubscriptionsAsync(int memberId)
        {
            var member = await unitOfWork.Members.GetAsync(m => m.Id == memberId);

            if (member == null)
                throw new NotFoundException($"This member is not found");

            var subs = await unitOfWork.Subscriptions.GetAllAsync(s => s.MemberId == memberId, new[] {"Member", "Plan"});

            return subs
                .OrderByDescending(s => s.StartDate)  
                .Select(MapToResponse)
                .ToList();
        }
    }
}
