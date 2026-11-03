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
        Task<Member> AddMemberAsync(MemberRequestDTO member);
        Task<IEnumerable<Member>> GetAllMembersAsync(string? searchTerm = null);

        Task<Member> GetMemberByIdAsync(int id);

        Task<Member> GetMemberProfileAsync(int id, bool WithSubscriptions = false, bool WithAttendances = false);

        Task<IEnumerable<Member>> GetMemberByPhoneOrNameAsync(string? searchTerm = null);

        Task<Member> UpdateMember(int id, MemberUpdateDTO memberDto);
        Task <Member> DeleteMember(int id);
    }
}
