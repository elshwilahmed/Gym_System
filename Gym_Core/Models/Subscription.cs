using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.Enums;

namespace Gym_Core.Models
{
    
    public class Subscription
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PaidAmount { get; set; }
        public int SessionsRemaining { get; set; }          
        public DateTime CreatedAt { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancelReason { get; set; }
        public SubscriptionStatus Status { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public int PlanId { get; set; }
        public MembershipPlan Plan { get; set; } = null!;

        public int CreatorId { get; set; }
        public AppUser Creator { get; set; } = null!;

        public int AttendanceId { get; set; }
        public IEnumerable<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
