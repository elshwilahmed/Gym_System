using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.Enums;

namespace Gym_Core.DTOs
{
    public class AuthRegisterResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public IEnumerable<EmployeeRole> Roles { get; set; } = new List<EmployeeRole>();
        public DateTime ExpiresOn { get; set; }
    }
}
