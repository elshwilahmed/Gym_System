using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;
using Gym_Core.Exceptions;
using Gym_Core.Interfaces;
using Gym_Core.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Gym_Core.Services                                  
{
    public class MembershipPlanService : IMembershipPlanService
    {
        private readonly IUnitOfWork unitOfWork;

        public MembershipPlanService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private static MembershipPlanResponseDto MapToResponse(MembershipPlan plan)
        {

            return new MembershipPlanResponseDto
            {
                Id = plan.Id,
                Name = plan.Name.Trim(),
                IsActive = plan.IsActive,
                Price = plan.Price,
                SessionsCount = plan.SessionsCount,
                DurationInDays = plan.DurationInDays,
                Description = plan.Description
            };

        }

        public async Task<MembershipPlanResponseDto> CreatePlanAsync(MembershipPlanDto model)
        {
            var Exist = await unitOfWork.Plans.GetAsync(p => p.Name.Trim() == model.Name.Trim());
            if (Exist != null)
            {
                if(Exist.IsActive == false)
                {
                    Exist.IsActive = true;
                    await unitOfWork.CompleteAsync();

                    return MapToResponse(Exist);
                }

                throw new ConflictException("This plan is already exist!");
            }

            var newPlan = new MembershipPlan
            {
                Name = model.Name.Trim(),
                DurationInDays = model.DurationInDays,
                IsActive = true,
                Price = model.Price,
                SessionsCount = model.SessionsCount,
                Description = model.Description
            };

            await unitOfWork.Plans.AddAsync(newPlan);
            await unitOfWork.CompleteAsync();

            return MapToResponse(newPlan);

        }

        public async Task<MembershipPlanResponseDto> DeletePlanAsync(int id)   // soft delete
        {
            var Exist = await unitOfWork.Plans.GetAsync(p => p.Id == id);
            if (Exist == null)
                throw new NotFoundException("This Plan wasn't Found!");

            Exist.IsActive = false;
            await unitOfWork.CompleteAsync();

            return MapToResponse(Exist);

        }

        public async Task<IEnumerable<MembershipPlanResponseDto>> GetAllPlansAsync(bool IncludingInActive)
        {
            var res = await unitOfWork.Plans.GetAllAsync(p => p.IsActive == true || p.IsActive == !IncludingInActive);

            var final = res.Select(MapToResponse);

            return final;
        }

        public async Task<MembershipPlanResponseDto> GetPlanByIdAsync(int id)
        {
            var Exist = await unitOfWork.Plans.GetAsync(p => p.Id == id);
            if (Exist == null)
                throw new NotFoundException("This Plan wasn't Found!");

            return MapToResponse(Exist);

        }

        public async Task<MembershipPlanResponseDto> UpdatePlanAsync(int id, MembershipPlanUpdateDTO model)
        {
            var Exist = await unitOfWork.Plans.GetAsync(p => p.Id == id);
            if (Exist == null)
                throw new NotFoundException("This Plan wasn't Found!");

            if (await unitOfWork.Plans.GetAsync(p => p.Name == model.Name.Trim() && p.Id != id) != null)
                throw new ConflictException("This plan name is already exist!");

            if (!string.IsNullOrWhiteSpace(model.Name))
                Exist.Name = model.Name.Trim();

            if (model.SessionsCount.HasValue)
                Exist.SessionsCount = model.SessionsCount;

            if(model.DurationInDays.HasValue)
                Exist.DurationInDays = model.DurationInDays;

            if(model.Price.HasValue)
                Exist.Price = model.Price;

            if(!string.IsNullOrEmpty(model.Description))
            Exist.Description = model.Description;

            await unitOfWork.CompleteAsync();

            return MapToResponse( Exist );


        }



    }
}
