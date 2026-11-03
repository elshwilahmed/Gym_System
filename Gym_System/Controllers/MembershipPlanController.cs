using Gym_Core.DTOs;
using Gym_Core.Interfaces;
using Gym_Core.Response;   
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Receptionist")]
    public class MembershipPlanController : ControllerBase
    {
        private readonly IMembershipPlanService planService;

        public MembershipPlanController(IMembershipPlanService planService)
        {
            this.planService = planService;
        }

        [HttpGet]
        [Route("GetAllPlans")]
        public async Task<ActionResult> GetAllPlans(bool includeInactive = false)
        {
            return Ok(APIResponse<IEnumerable<MembershipPlanResponseDto>>
                .Success(await planService.GetAllPlansAsync(includeInactive)));
        }

        [HttpGet]
        [Route("GetPlan/{id:int}")]
        public async Task<ActionResult> GetPlanById(int id)
        {
            return Ok(APIResponse<MembershipPlanResponseDto>
                .Success(await planService.GetPlanByIdAsync(id)));
        }

        [HttpPost]
        [Route("AddPlan")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddPlan(MembershipPlanDto model)
        {
            return Ok(APIResponse<MembershipPlanResponseDto>
                .Success(await planService.CreatePlanAsync(model)));
        }

        [HttpPut]
        [Route("UpdatePlan/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdatePlan(int id, MembershipPlanUpdateDTO model)
        {
            return Ok(APIResponse<MembershipPlanResponseDto>
                .Success(await planService.UpdatePlanAsync(id, model)));
        }

        [HttpDelete]
        [Route("DeletePlan/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeletePlan(int id)
        {
            return Ok(APIResponse<MembershipPlanResponseDto>
                .Success(await planService.DeletePlanAsync(id)));
        }
    }
}