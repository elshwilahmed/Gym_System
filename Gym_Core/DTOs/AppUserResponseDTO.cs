using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.Enums;
using Gym_Core.Models;

namespace Gym_Core.DTOs
{
    public class AppUserResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;
        public bool IsActive { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
