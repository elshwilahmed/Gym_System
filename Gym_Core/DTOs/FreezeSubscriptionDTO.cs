using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class FreezeSubscriptionDTO
    {
        [Range(7, 30)]
        public int Days { get; set; }

        [MaxLength(300)]
        public string? Reason { get; set; }
    }
}
