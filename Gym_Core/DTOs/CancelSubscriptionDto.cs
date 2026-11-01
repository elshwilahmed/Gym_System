using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class CancelSubscriptionDto
    {
        [Range(0, 1000000)]
        public decimal RefundAmount { get; set; }

        [Required, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}
