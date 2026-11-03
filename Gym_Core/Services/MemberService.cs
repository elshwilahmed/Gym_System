using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;
using Gym_Core.Exceptions;
using Gym_Core.Interfaces;
using Gym_Core.Models;

namespace Gym_Core.Services
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork unitOfWork;

        public MemberService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Member> AddMemberAsync(MemberRequestDTO member)
        {
            var existingMember = await unitOfWork.Members.GetAsync(m => m.Phone == member.Phone);

            if (existingMember != null)
            {
                if (existingMember.IsActive)
                    throw new ConflictException($"This phone number whose {existingMember.FirstName} is already exist!");

                existingMember.IsActive = true;
                await unitOfWork.CompleteAsync();
                return existingMember;
            }

            if (!member.DateOfBirth.HasValue)   // nullable in the member request but required in the member model
                throw new BadRequestException("Date of birth is required for new members!");


            var newMember = new Member
            {
                FirstName = member.FirstName,
                MiddleName = member.MiddleName,
                LastName = member.LastName,
                DateOfBirth = member.DateOfBirth.Value,
                Phone = member.Phone,
                IsActive = true,
            };

            await unitOfWork.Members.AddAsync(newMember);
            await unitOfWork.CompleteAsync();

            return newMember;
        }

        public async Task<Member> DeleteMember(int id)    // soft delete
        {
            var member = await unitOfWork.Members.GetAsync(m=>m.Id == id);

            if (member == null)
                throw new NotFoundException("The member is not found");

            member.IsActive = false;

            await unitOfWork.CompleteAsync();
            return member;
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync(string? searchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await unitOfWork.Members.GetAllAsync(m => m.IsActive == true);

            searchTerm = searchTerm!.Trim();

            var query = await unitOfWork.Members.GetAllAsync(m => (
                                                            m.FullName.Contains(searchTerm) ||
                                                            m.Phone.Contains(searchTerm)
                                                            )  
                                                            && m.IsActive==true);
            return query;
        }

        public async Task<Member> GetMemberByIdAsync(int id)
        {
            var member = await unitOfWork.Members.GetAsync(m => m.Id == id);

            if (member == null)
                throw new NotFoundException($"This member is not found");

            return member;
        } 

        public async Task<IEnumerable<Member>> GetMemberByPhoneOrNameAsync(string? searchTerm = null) // return all and receptionest will choose which need 
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllMembersAsync(searchTerm);
            searchTerm = searchTerm.Trim();

            var query = await unitOfWork.Members.GetAllAsync(m => m.Phone.Contains(searchTerm) ||
                                                               m.FullName.Contains(searchTerm));
            if (!query.Any())
                throw new NotFoundException("This member is not found");

            return query;
        }

        public async Task<Member> GetMemberProfileAsync(int id, bool WithSubscriptions = false, bool WithAttendances = false)   // with details
        {
            var includes = new List<string>();

            if (WithSubscriptions is true) includes.Add("Subscriptions");
            if (WithAttendances is true) includes.Add("Attendances");

            var query = await unitOfWork.Members.GetAsync( m => m.Id == id, includes.ToArray());

            if (query == null)
                throw new NotFoundException("This member is not found");

            return query;
        }

        public async Task<Member> UpdateMember(int id, MemberUpdateDTO memberDto)
        {
            var existingMember = await unitOfWork.Members.GetAsync(m => m.Id == id);

            if (existingMember == null)
                throw new NotFoundException("The member is not found");

            if (!string.IsNullOrWhiteSpace(memberDto.FirstName))
                existingMember.FirstName = memberDto.FirstName.Trim();

            if (!string.IsNullOrWhiteSpace(memberDto.MiddleName))
                existingMember.MiddleName = memberDto.MiddleName.Trim();

            if (!string.IsNullOrWhiteSpace(memberDto.LastName))
                existingMember.LastName = memberDto.LastName.Trim();

            if (!string.IsNullOrWhiteSpace(memberDto.Phone))
            {
                if (existingMember.Phone != memberDto.Phone)
                {
                    var phoneExists = await unitOfWork.Members.GetAsync(m => m.Phone == memberDto.Phone.Trim());
                    if (phoneExists != null)
                        throw new ConflictException("This phone was already used");

                    existingMember.Phone = memberDto.Phone.Trim();
                }
            }

            if (memberDto.DateOfBirth.HasValue)
                existingMember.DateOfBirth = memberDto.DateOfBirth.Value;

            await unitOfWork.CompleteAsync();
            return existingMember;
        }
    }
}
