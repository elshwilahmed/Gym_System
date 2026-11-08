using System.Reflection.Emit;
using Gym_Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gym_Infrastructure.Data
{
    public class GymDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MembershipPlan>().Property(p => p.Price).HasPrecision(18, 2);
            builder.Entity<Subscription>().Property(s => s.PaidAmount).HasPrecision(18, 2);

            builder.Entity<Member>()
            .Property(m => m.FullName)
            .HasComputedColumnSql("CONCAT_WS(' ', [FirstName], NULLIF([MiddleName], ''), [LastName])");

            builder.Entity<Member>().HasIndex(m => m.Phone).IsUnique();
            builder.Entity<MembershipPlan>().HasIndex(p => p.Name).IsUnique();

            builder.Entity<Attendance>()
            .HasOne(a => a.Subscription).WithMany(s => s.Attendances)
            .HasForeignKey(a => a.SubscriptionId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RefreshToken>().HasIndex(r => r.TokenHash).IsUnique();

            builder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int>()
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole<int>()
                {
                    Id = 2,
                    Name = "Receptionist",
                    NormalizedName = "RECEPTIONIST",
                    ConcurrencyStamp = "2"
                }
                );
        }
    }
}
