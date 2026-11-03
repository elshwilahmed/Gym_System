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
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork unitOfWork;

        public AttendanceService(IUnitOfWork unitOfWork)
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
        }

        public async Task<AttendanceResponseDto> CheckInAsync(int id)
        {
            var ExistingMember = await unitOfWork.Members.GetAsync(m => m.Id == id);
            if (ExistingMember == null)
                throw new NotFoundException("This member wasn't found");
            if (!ExistingMember.IsActive)
                throw new BadRequestException("This member is not active");

            var subs = await unitOfWork.Subscriptions
                       .GetAllAsync(s => s.MemberId == id && s.Status == SubscriptionStatus.Active, new[] { "Plan" });
            var subscription = subs.FirstOrDefault(s => GetEffectiveStatus(s) == SubscriptionStatus.Active);

            if (subscription == null)
                throw new BadRequestException("This member not subscripe in any membership plan");

            var today = DateTime.UtcNow.Date;

            var already = await unitOfWork.Attendances.GetAsync(
                     a => a.MemberId == id && a.CheckInTime >= today && a.CheckInTime < today.AddDays(1));
            if (already != null)
                throw new ConflictException($"Already checked in today at {already.CheckInTime:HH:mm}");

            var attendance = new Attendance
            {
                CheckInTime = DateTime.UtcNow,
                MemberId = id,
                Member = ExistingMember,
                Subscription = subscription,
                SubscriptionId = subscription.Id
            };

            await unitOfWork.Attendances.AddAsync(attendance);
            if (subscription.Plan.SessionsCount > 0)     // if there an unlimit plan
                subscription.SessionsRemaining -= 1;

            await unitOfWork.CompleteAsync();

            string? warning = null;
            var daysLeft = (subscription.EndDate - today).Days;

            if (subscription.Plan.SessionsCount > 0 && subscription.SessionsRemaining <= 3)
                warning = subscription.SessionsRemaining == 0
                    ? "This was the last session. Please renew."
                    : $"{subscription.SessionsRemaining} session(s) left.";
            else if (daysLeft <= 3)
                warning = $"Subscription ends in {daysLeft} day(s).";

            return new AttendanceResponseDto
            {
                MemberName = ExistingMember.FullName,
                MemberId = ExistingMember.Id,
                PlanName = subscription.Plan.Name,
                CheckInTime = attendance.CheckInTime,
                SessionsRemaining = subscription.SessionsRemaining,
                Id = attendance.Id,
                SubscriptionId = subscription.Id,
                Warning = warning
            };

        }


        public async Task CancelCheckInAsync(int id)
        {
            var attendance = await unitOfWork.Attendances.GetAsync(
                             a => a.Id == id, new[] { "Subscription.Plan" });

            if (attendance == null)
                throw new NotFoundException("This attendance wasn't found");

            var today = DateTime.UtcNow.Date;
            if (attendance.CheckInTime < today)
                throw new BadRequestException("Only today's check-ins can be cancelled!");

            if (attendance.Subscription.Plan.SessionsCount > 0)
                attendance.Subscription.SessionsRemaining++;

            await unitOfWork.Attendances.DeleteById(attendance.Id);
            await unitOfWork.CompleteAsync();
        }


        public async Task<ReportResponseDTO> GetReportAsync(AttendanceReportQueryDto query)
        {
            var from = query.From?.Date ?? DateTime.UtcNow.Date.AddDays(-30);
            var to = query.To?.Date.AddDays(1) ?? DateTime.UtcNow.AddDays(1).Date;

            var attendances = await unitOfWork.Attendances.
                GetAllAsync(a => a.CheckInTime >= from && a.CheckInTime < to &&
                             (query.MemberId == null || a.MemberId == query.MemberId),  // one member or all (2 cases)
                                             new[] { "Member", "Subscription.Plan" });

            var membersAttendances = attendances.Select(
            a => new AttendanceResponseDto
            {
                CheckInTime = a.CheckInTime,
                SubscriptionId = a.SubscriptionId,
                Id = a.Id,
                MemberId = a.MemberId,
                MemberName = a.Member.FullName,
                PlanName = a.Subscription.Plan.Name
            }
                ).OrderByDescending(x => x.CheckInTime).ToList();

            return new ReportResponseDTO
            {
                TotalVisits = membersAttendances.Count(),
                UniqueMembers = membersAttendances.Select(x => x.MemberId).Distinct().Count(),
                MembersAttendances = membersAttendances
            };
        }


    }
}