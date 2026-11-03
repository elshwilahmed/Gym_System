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
            return Ok(APIResponse<Member>.Success(await memberService.AddMemberAsync(member)));
        }

        [HttpDelete]
        [Route("DeleteMember/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMember(int id)
        {
            return Ok(APIResponse<Member>.Success(await memberService.DeleteMember(id)));
        }


        [HttpGet]
        [Route("GetMemberByPhoneOrName/{searchTerm?}")]
        public async Task<ActionResult> GetMemberByPhoneOrNameAsync(string? searchTerm)
        {
            return Ok(APIResponse<IEnumerable<Member>>.Success(await memberService.GetMemberByPhoneOrNameAsync(searchTerm)));
        }

        [HttpGet]
        [Route("GetMemberProfile/{id:int}")]
        public async Task<ActionResult> GetMemberProfileAsync(int id, bool WithSubscriptions, bool WithAttendances)
        {
            return Ok(APIResponse<Member>.Success(await memberService.GetMemberProfileAsync(id, 
                                                                            WithSubscriptions, WithAttendances)));
        }

        [HttpPut]
        [Route("UpdateMember/{id:int}")]
        public async Task<ActionResult> UpdateMember(int id, MemberUpdateDTO memberDto)
        {
            return Ok(APIResponse<Member>.Success(await memberService.UpdateMember(id, memberDto)));
        }
    }
}
