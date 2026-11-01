using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateTime  CheckInTime { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = null!;
    }
}
