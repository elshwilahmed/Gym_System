using System.Text;
using Gym_Core.DTOs;
using Gym_Core.Enums;
using Gym_Core.Exceptions;
using Gym_Core.Interfaces;
using Gym_Core.Models;
using Gym_Core.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gym_Core.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IJWTService jwt;
        private readonly JWTsettings options;
        private readonly IUnitOfWork unitOfWork;

        public AppUserService(UserManager<AppUser> userManager, IJWTService jwt,
                                IOptions<JWTsettings> options, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.jwt = jwt;
            this.options = options.Value;
            this.unitOfWork = unitOfWork;
        }


        private static AppUserResponseDTO MapToUserResponse(AppUser user)
        {
            return new AppUserResponseDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                UserName = user.LastName,
                LastName = user.LastName,
                Phone = user.Phone,
                IsActive = user.IsActive,
                Email = user.Email!
            };
        }

        public async Task<AuthRegisterResponseDTO> AddEmployee(AuthRegisterDTO model)
        {
            var employee = await userManager.FindByEmailAsync(model.Email);
            if (employee is not null)
            {
                throw new ConflictException(employee.IsActive
                                    ? "This email already exists."
                                    : "This email belongs to a disabled employee. Activate it instead.");
            }

            var employeeByName = await userManager.FindByNameAsync(model.UserName);
            if (employeeByName is not null)
            {
                throw new ConflictException(employeeByName.IsActive
                                         ? "This name already exists."
                                         : "This name belongs to a disabled employee. Activate it instead.");
            }

            var newUser = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsActive = true,
                Phone = model.Phone
            };

            var res = await userManager.CreateAsync(newUser, model.Password);

            if (!res.Succeeded)
            {
                var errors = new StringBuilder();

                foreach (var error in res.Errors)
                {
                    errors.AppendLine(error.Description);
                }

                throw new BadRequestException(errors.ToString());                
            }

            var roleResult = await userManager.AddToRoleAsync(newUser, model.Role.ToString());
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(newUser);   
                throw new BadRequestException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            return new AuthRegisterResponseDTO()
            {
                IsAuthenticated = true,
                Message = "User registered successfully!",
                Email = newUser.Email,
                FullName = model.FirstName + " " + model.LastName,
                Roles = new List<EmployeeRole> { model.Role },
                ExpiresOn = DateTime.UtcNow.AddMinutes(options.ExpiryInMinutes)
            };

        }

        public async Task<AppUserResponseDTO> DeleteEmployee(int id)
        {
            var employee = await userManager.FindByIdAsync($"{id}");

            if (employee is null)
                throw new NotFoundException("This employee is not found!");
            if (employee.Email == "admin@gym.com") 
            {
                throw new BadRequestException("You cannot delete the main system administrator!");
            }



            employee.IsActive = false;

            var tokens = await unitOfWork.RefreshTokens.GetAllAsync(
                    t => t.UserId == employee.Id && t.RevokedOn == null);

            foreach (var t in tokens)
                t.RevokedOn = DateTime.UtcNow;

            await unitOfWork.CompleteAsync();

            var result = await userManager.UpdateAsync(employee);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return MapToUserResponse(employee);
        }

        public async Task<AppUserResponseDTO> ActivateEmployee(int id)
        {
            var employee = await userManager.FindByIdAsync($"{id}");

            if (employee is null)
                throw new NotFoundException("This employee is not found!");

            if (employee.IsActive)
                throw new BadRequestException("This employee is already active.");

            employee.IsActive = true;

            var result = await userManager.UpdateAsync(employee);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return MapToUserResponse(employee);
        }

        public async Task<IEnumerable<AppUserResponseDTO>> GetAllEmployees()
        {
            var employees = await userManager.Users.ToListAsync();

            var result = new List<AppUserResponseDTO>();

            foreach (var e in employees)
            {
                var dto = MapToUserResponse(e);
                dto.Role = (await userManager.GetRolesAsync(e)).FirstOrDefault() ?? "";
                result.Add(dto);
            }

            return result;
        }

        public async Task<AppUserResponseDTO> GetEmployeeById(int id)
        {
            var employee = await userManager.FindByIdAsync($"{id}");

            if (employee is null)
                throw new NotFoundException("The Employee is not found!");

            var dto = MapToUserResponse(employee);
            dto.Role = (await userManager.GetRolesAsync(employee)).FirstOrDefault() ?? "";

            return dto;
        }

        public async Task<AppUserResponseDTO> UpdateEmployeeById(int id, UpdateUserDTO model)
        {
            var existingUser = await userManager.FindByIdAsync($"{id}");

            if (existingUser == null)
                throw new NotFoundException("The user is not found");

            if (!string.IsNullOrWhiteSpace(model.UserName))
                existingUser.UserName = model.UserName.Trim();

            if (!string.IsNullOrWhiteSpace(model.Email))
                existingUser.Email = model.Email.Trim();

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var passwordHasher = new PasswordHasher<AppUser>();
                existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, model.Password.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                if (existingUser.Phone != model.Phone)
                {
                    var phoneExists = userManager.Users.Any(m => m.Phone == model.Phone.Trim() && m.Id != id);
                    if (phoneExists != false)
                        throw new ConflictException("This phone was already used");

                    existingUser.Phone = model.Phone.Trim();
                }
            }
            await userManager.UpdateAsync(existingUser);
            return MapToUserResponse(existingUser);
        }
    }
}
