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
        Task<AppUser> DeleteEmployee(int id);
        Task<IEnumerable<AppUser>> GetAllEmployees();
        Task<AppUser> GetEmployeeById(int id);
        Task<AppUser> UpdateEmployeeById(int id, UpdateUserDTO model);


    }
}
