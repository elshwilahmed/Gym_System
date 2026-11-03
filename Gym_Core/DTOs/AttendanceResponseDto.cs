using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Gym_Core.DTOs
{
    public class AttendanceResponseDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public int SubscriptionId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public int? SessionsRemaining { get; set; }
        public string? Warning { get; set; } = string.Empty;
    }
}
