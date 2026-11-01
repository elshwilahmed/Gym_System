using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;

namespace Gym_Core.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponseDto> CreateAsync(CreateSubscriptionDto dto, int creatorId);
        Task<SubscriptionResponseDto> CancelAsync(int id, CancelSubscriptionDto dto);
        Task<SubscriptionResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<SubscriptionResponseDto>> GetAllSubscriptionsAsync(bool IncludingExpired = false);
        Task<IEnumerable<SubscriptionResponseDto>> GetMemberSubscriptionsAsync(int memberId);
    }
}
