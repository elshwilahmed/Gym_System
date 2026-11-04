using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class MemberProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public bool IsActive { get; set; }
        public List<SubscriptionResponseDto>? Subscriptions { get; set; }
        public List<AttendanceResponseDto>? Attendances { get; set; }
    }
}
