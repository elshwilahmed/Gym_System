using Gym_Core.Interfaces;
using Gym_Core.Models;
using Gym_Infrastructure.Data;

namespace Gym_Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _context;

        public IGeniricRepository<Member> Members { get; private set; }
        public IGeniricRepository<Subscription> Subscriptions { get; private set; }
        public IGeniricRepository<MembershipPlan> Plans { get; private set; }
        public IGeniricRepository<Attendance> Attendances { get; private set; }
        public IGeniricRepository<RefreshToken> RefreshTokens { get; private set; }

        public UnitOfWork(GymDbContext context)
        {
            _context = context;

            Members = new GenericRepository<Member>(_context);
            Subscriptions = new GenericRepository<Subscription>(_context);
            Plans = new GenericRepository<MembershipPlan>(_context);
            Attendances = new GenericRepository<Attendance>(_context);
            RefreshTokens = new GenericRepository<RefreshToken>(_context);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}