using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.DTOs;
using Gym_Core.Exceptions;
using Gym_Core.Interfaces;
using Gym_Core.Models;
using Gym_Core.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Gym_Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IJWTService jwt;
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly JWTsettings options;

        public AuthService(UserManager<AppUser> userManager, IOptions<JWTsettings> options,
                            IJWTService jwt, RoleManager<IdentityRole<int>> roleManager)
        {
            this.userManager = userManager;
            this.jwt = jwt;
            this.roleManager = roleManager;
            this.options = options.Value;
        }



        public async Task<AuthLoginResponseDTO> LoginAsync(AuthLoginDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null)
                throw new BadRequestException("Incorrect Email Or password!");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                throw new BadRequestException("Incorrect Email Or password!");

            var jwtToken = await jwt.GenerateJwtTokenAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            return new AuthLoginResponseDTO()
            {
                IsAuthenticated = true,
                Email = user.Email!,
                Roles = userRoles.ToList(),
                Token = jwtToken,
                ExpiresOn = DateTime.UtcNow.AddDays(options.ExpiryInDays) 
            };
        }

        public async Task<IEnumerable<string>> GetAllRoles()
        {
            var all = await roleManager.Roles.Select(r => r.Name).ToListAsync();

            return all!;
        }
    }
}
