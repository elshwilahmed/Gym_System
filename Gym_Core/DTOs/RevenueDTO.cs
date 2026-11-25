using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class RevenueDTO
    {
        public decimal TotalPaid { get; set; }
        public decimal TotalRefunded { get; set; }
        public decimal Net => TotalPaid - TotalRefunded;
        public int SubscriptionsCount { get; set; }
    }
}
