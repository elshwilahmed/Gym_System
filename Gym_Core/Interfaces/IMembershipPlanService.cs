using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;

namespace Gym_Core.Interfaces
{
    public interface IMembershipPlanService
    {
        Task<IEnumerable<MembershipPlanResponseDto>> GetAllPlansAsync(bool IncludingInActive = false);
        Task<MembershipPlanResponseDto> GetPlanByIdAsync(int id);
        Task<MembershipPlanResponseDto> CreatePlanAsync(MembershipPlanDto model);
        Task<MembershipPlanResponseDto> UpdatePlanAsync(int id, MembershipPlanUpdateDTO model);
        Task<MembershipPlanResponseDto> DeletePlanAsync(int id);
    }
}
