using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class UpdateUserDTO
    {
        [Required, StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(11)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
