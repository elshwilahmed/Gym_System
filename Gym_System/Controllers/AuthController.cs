using Gym_Core.DTOs;
using Gym_Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gym_Core.Response;
using Microsoft.AspNetCore.Authorization;

namespace Gym_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }
       
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> LoginAsync(AuthLoginDTO loginDTO)
        {
            var res = await authService.LoginAsync(loginDTO);

            if (res.IsAuthenticated == false)
                return BadRequest(APIResponse<AuthResponseDTO>.Failure(res.Message));

            return Ok(APIResponse<AuthResponseDTO>.Success(res));
        }
    }
}
