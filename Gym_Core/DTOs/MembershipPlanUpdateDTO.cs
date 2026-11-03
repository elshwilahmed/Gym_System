using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class MembershipPlanUpdateDTO
    {
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 3650)]
        public int? DurationInDays { get; set; }

        [Range(0, 10000)]
        public int? SessionsCount { get; set; }

        [Range(0, 1000000)]
        public decimal? Price { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
