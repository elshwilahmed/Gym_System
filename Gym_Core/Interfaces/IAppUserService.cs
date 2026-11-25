using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;
using Gym_Core.Models;

namespace Gym_Core.Interfaces
{
    public interface IAppUserService
    {
        Task<AuthRegisterResponseDTO> AddEmployee(AuthRegisterDTO model);
        Task<AppUserResponseDTO> DeleteEmployee(int id);
        Task<IEnumerable<AppUserResponseDTO>> GetAllEmployees();
        Task<AppUserResponseDTO> GetEmployeeById(int id);
        Task<AppUserResponseDTO> UpdateEmployeeById(int id, UpdateUserDTO model);
        Task<AppUserResponseDTO> ActivateEmployee(int id);
        Task<bool> ResetEmployeePassword(int id, ResetPassDTO model);

    }
}
