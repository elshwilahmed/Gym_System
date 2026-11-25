using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class ResetPassDTO
    {
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
