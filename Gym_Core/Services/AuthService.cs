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
        private readonly IUnitOfWork unitOfWork;
        private readonly JWTsettings options;

        public AuthService(UserManager<AppUser> userManager, IOptions<JWTsettings> options,
                            IJWTService jwt, RoleManager<IdentityRole<int>> roleManager, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.jwt = jwt;
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
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

            var refreshToken = jwt.GenerateRefreshToken();
            var refreshTokenHash = jwt.Hash(refreshToken);

            await unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,     
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(options.RefrshTokenExpiryInDays)
            });
            await unitOfWork.CompleteAsync();

            return new AuthLoginResponseDTO()
            {
                IsAuthenticated = true,
                Email = user.Email!,
                Roles = userRoles.ToList(),
                Token = jwtToken,
                RefreshToken = refreshToken,
                ExpiresOn = DateTime.UtcNow.AddMinutes(options.ExpiryInMinutes)
            };
        }

        public async Task<IEnumerable<string>> GetAllRoles()
        {
            var all = await roleManager.Roles.Select(r => r.Name).ToListAsync();

            return all!;
        }

        public async Task<AuthLoginResponseDTO> Refresh(RefreshRequestDTO dto)
        {
            var hashToken = jwt.Hash(dto.refreshToken);
            var stored = await unitOfWork.RefreshTokens.GetAsync(r => r.TokenHash == hashToken, new[] {"User"});

            if (stored == null || stored.IsActive == false)
                throw new UnauthorizedException("Invalid Refresh Token");

            var response = new AuthLoginResponseDTO()
            {
                Email = stored.User.Email!,
                FullName = stored.User.FirstName + " " + stored.User.LastName,
                Token = await jwt.GenerateJwtTokenAsync(stored.User),
                RefreshToken = jwt.GenerateRefreshToken(),
                IsAuthenticated = true,
                ExpiresOn = DateTime.UtcNow.AddMinutes(options.ExpiryInMinutes),
                Roles = await userManager.GetRolesAsync(stored.User)
            };

            await unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            {
                CreatedOn = DateTime.UtcNow,
                TokenHash = jwt.Hash(response.RefreshToken), 
                ExpiresOn = DateTime.UtcNow.AddDays(options.RefrshTokenExpiryInDays) ,
                UserId = stored.User.Id
            });
            stored.RevokedOn = DateTime.UtcNow;

            await unitOfWork.CompleteAsync();

            return response;
        }

        public async Task LogoutAsync(RefreshRequestDTO dto)
        {
            var hash = jwt.Hash(dto.refreshToken);
            var stored = await unitOfWork.RefreshTokens.GetAsync(r => r.TokenHash == hash);

            if (stored == null || !stored.IsActive)
                return;

            stored.RevokedOn = DateTime.UtcNow;
            await unitOfWork.CompleteAsync();
        }
    }
}
