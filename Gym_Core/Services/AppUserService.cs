using System.Text;
using Gym_Core.DTOs;
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

        public AppUserService(UserManager<AppUser> userManager, IJWTService jwt, IOptions<JWTsettings> options)
        {
            this.userManager = userManager;
            this.jwt = jwt;
            this.options = options.Value;
        }

        public async Task<AuthResponseDTO> AddEmployee(AuthRegisterDTO model)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not null)
                throw new ConflictException ("is email is already exist!" );
            if (await userManager.FindByNameAsync(model.UserName) is not null)
                throw new ConflictException("This username is already exist!" );

            var newUser = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var res = await userManager.CreateAsync(newUser, model.Password);

            if (!res.Succeeded)
            {
                var errors = new StringBuilder();

                foreach (var error in res.Errors)
                {
                    errors.AppendLine(error.Description);
                }

                return new AuthResponseDTO()
                {
                    Message = errors.ToString()
                };
            }

            await userManager.AddToRoleAsync(newUser, model.Role);

            return new AuthResponseDTO()
            {
                IsAuthenticated = true,
                Message = "User registered successfully!",
                Email = newUser.Email,
                Roles = new List<string> { model.Role },
                ExpiresOn = DateTime.UtcNow.AddDays(options.ExpiryInDays)
            };

        }

        public async Task<AppUser> DeleteEmployee(int id)
        {
            var employee = await userManager.FindByIdAsync($"{id}");

            if (employee is null)
                throw new NotFoundException("This employee is not found!");
            if (employee.Email == "admin@gym.com") 
            {
                throw new BadRequestException("You cannot delete the main system administrator!");
            }

            await userManager.DeleteAsync(employee);

            return employee;
        }

        public async Task<IEnumerable<AppUser>> GetAllEmployees()
        {
            return await userManager.Users.ToListAsync();
        }

        public async Task<AppUser> GetEmployeeById(int id)
        {
            var employee = await userManager.FindByIdAsync($"{id}");

            if (employee is null)
                throw new NotFoundException("The Employee is not found!");

            return employee;
        }

        public async Task<AppUser> UpdateEmployeeById(int id, UpdateUserDTO model)
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
                    var phoneExists = userManager.Users.Any(m => m.Phone == model.Phone.Trim());
                    if (phoneExists != false)
                        throw new ConflictException("This phone was already used");

                    existingUser.Phone = model.Phone.Trim();
                }
            }
            await userManager.UpdateAsync(existingUser);
            return existingUser;
        }
    }
}
