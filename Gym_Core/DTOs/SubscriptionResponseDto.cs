using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.Enums;

namespace Gym_Core.DTOs
{
    public class SubscriptionResponseDto
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;

        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public int SessionsRemaining { get; set; }

        public SubscriptionStatus Status { get; set; }
    }
}
