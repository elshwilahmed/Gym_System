using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Core.DTOs
{
    public class AuthRegisterDTO
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [EmailAddress(ErrorMessage = "Email must contain a . and @")]
        [RegularExpression(@".*\..*", ErrorMessage = "Email must contain a . and @")]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(11)]
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "Invalid egyption phone")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
