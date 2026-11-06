using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;

namespace Gym_Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthLoginResponseDTO> LoginAsync(AuthLoginDTO model);
        Task<IEnumerable<string>> GetAllRoles();

    }
}
