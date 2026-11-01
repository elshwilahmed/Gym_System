using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class CreateSubscriptionDto
    {
        [Range(1, int.MaxValue)]
        public int MemberId { get; set; }

        [Range(1, int.MaxValue)]
        public int PlanId { get; set; }

    }
}
