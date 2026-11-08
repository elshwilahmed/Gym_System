using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gym_Core.Interfaces;
using Gym_Core.Models;

namespace Gym_Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGeniricRepository <Member> Members { get; }
        IGeniricRepository <MembershipPlan> Plans { get; }
        IGeniricRepository <Subscription> Subscriptions { get; }
        IGeniricRepository<Attendance> Attendances { get; }
        IGeniricRepository<RefreshToken> RefreshTokens { get; }

        public Task CompleteAsync(); 


    }
}
