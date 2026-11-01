using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class MemberRequestDTO
    {
        [Required, MaxLength(15)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(15)]
        public string MiddleName { get; set; } = string.Empty;

        [Required, MaxLength(15)]
        public string LastName { get; set; } = String.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
    }
}
