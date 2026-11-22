using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Gym_Core.DTOs;
using Gym_Core.Models;

namespace Gym_Core.Interfaces
{
    public interface IMemberService
    {
        Task<MemberResponseDTO> AddMemberAsync(MemberRequestDTO member);
        Task<PagedResultDTO<MemberResponseDTO>> GetAllMembersAsync(int page, string? searchTerm = null);

        Task<MemberResponseDTO> GetMemberByIdAsync(int id);

        Task<MemberProfileDto> GetMemberProfileAsync(int id, bool WithSubscriptions = false, bool WithAttendances = false);

        Task<MemberResponseDTO> UpdateMember(int id, MemberUpdateDTO memberDto);
        Task <MemberResponseDTO> DeleteMember(int id);
    }
}
