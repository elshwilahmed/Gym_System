using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.Models;

namespace Gym_Core.Interfaces
{
    public interface IJWTService
    {
        Task<string> GenerateJwtTokenAsync(AppUser user);
        string GenerateRefreshToken();
        string Hash(string refreshToken);
    }
}
