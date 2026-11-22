using Gym_Core.DTOs;
using Gym_Core.Interfaces;
using Gym_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Gym_Core.Response;
using Microsoft.AspNetCore.Authorization;
namespace Gym_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Receptionist")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService memberService;

        public MemberController(IMemberService memberService)
        {
            this.memberService = memberService;
        }

        [HttpPost]
        [Route("AddMember")]
        public async Task<ActionResult> AddMember(MemberRequestDTO member)
        {
            return Ok(APIResponse<MemberResponseDTO>.Success(await memberService.AddMemberAsync(member)));
        }

        [HttpDelete]
        [Route("DeleteMember/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMember(int id)
        {
            return Ok(APIResponse<MemberResponseDTO>.Success(await memberService.DeleteMember(id)));
        }

        [HttpGet]
        [Route("GetAllMembers")]
        public async Task<ActionResult> GetAllMembers([FromQuery] int page = 1, [FromQuery] string? search = null)
        {
            return Ok(APIResponse<PagedResultDTO<MemberResponseDTO>>
                .Success(await memberService.GetAllMembersAsync(page, search)));
        }

        [HttpGet]
        [Route("GetMemberProfile/{id:int}")]
        public async Task<ActionResult> GetMemberProfileAsync(int id, bool WithSubscriptions, bool WithAttendances)
        {
            return Ok(APIResponse<MemberProfileDto>.Success(await memberService.GetMemberProfileAsync(id, 
                                                                            WithSubscriptions, WithAttendances)));
        }

        [HttpPut]
        [Route("UpdateMember/{id:int}")]
        public async Task<ActionResult> UpdateMember(int id, MemberUpdateDTO memberDto)
        {
            return Ok(APIResponse<MemberResponseDTO>.Success(await memberService.UpdateMember(id, memberDto)));
        }
    }
}
