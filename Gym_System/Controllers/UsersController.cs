using Gym_Core.DTOs;
using Gym_Core.Interfaces;
using Gym_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gym_Core.Response;

namespace Gym_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class UsersController : ControllerBase
    {
        private readonly IAppUserService _appUserService;

        public UsersController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpPost("Employee")]
        public async Task<IActionResult> AddEmployee(AuthRegisterDTO model)    
        {
            var result = await _appUserService.AddEmployee(model);

            if (!result.IsAuthenticated)
                return BadRequest(APIResponse<AuthResponseDTO>.Failure(result.Message));

            return Ok(APIResponse<AuthResponseDTO>.Success(result));
        }

        [HttpGet("AllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _appUserService.GetAllEmployees();
            return Ok(APIResponse<IEnumerable<AppUser>>.Success(employees));
        }

        [HttpGet("Employee/{id:int}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _appUserService.GetEmployeeById(id);

            return Ok(APIResponse<AppUser>.Success(employee));

        }

        [HttpDelete("DeleteEmployee/{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {

            var deletedEmployee = await _appUserService.DeleteEmployee(id);

            return Ok(APIResponse<string>.Success(null!, msg:"The employee deleted successfully"));
        }
    }
}
