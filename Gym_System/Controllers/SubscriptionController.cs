using System.Security.Claims;
using Gym_Core.DTOs;
using Gym_Core.Interfaces;
using Gym_Core.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gym_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Receptionist")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            this.subscriptionService = subscriptionService;
        }

        [HttpPost]
        [Route("AddSubscription")]
        public async Task<ActionResult> AddSubscription(CreateSubscriptionDto dto)
        {
            var creatorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            return Ok(APIResponse<SubscriptionResponseDto>
                .Success(await subscriptionService.CreateAsync(dto, creatorId),
                         "Subscription created successfully"));
        }

        [HttpDelete]
        [Route("CancelSubscription/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CancelSubscription(int id, CancelSubscriptionDto dto)
        {
            return Ok(APIResponse<SubscriptionResponseDto>
                .Success(await subscriptionService.CancelAsync(id, dto),
                         "Subscription cancelled successfully"));
        }

        [HttpGet]
        [Route("GetSubscription/{id:int}")]
        public async Task<ActionResult> GetSubscriptionById(int id)
        {
            return Ok(APIResponse<SubscriptionResponseDto>
                .Success(await subscriptionService.GetByIdAsync(id)));
        }

        [HttpGet]
        [Route("GetMemberSubscriptions/{memberId:int}")]
        public async Task<ActionResult> GetMemberSubscriptions(int memberId)
        {
            return Ok(APIResponse<IEnumerable<SubscriptionResponseDto>>
                .Success(await subscriptionService.GetMemberSubscriptionsAsync(memberId)));
        }

        [HttpGet]
        [Route("GetAllSubscriptions")]
        public async Task<ActionResult> GetAllSubscriptions([FromQuery] bool includeExpired = false)
        {
            return Ok(APIResponse<IEnumerable<SubscriptionResponseDto>>
                .Success(await subscriptionService.GetAllSubscriptionsAsync(includeExpired)));
        }
    }
}


