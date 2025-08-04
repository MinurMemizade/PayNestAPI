using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayNestAPI.Models.Entities;
using PayNestAPI.Models.Security;

namespace PayNestAPI.Context
{
    public class AppDBContext : IdentityDbContext<AppUser, Roles, Guid>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(c=>c.Cards)
                .WithOne(u=>u.User)
                .HasForeignKey(u=>u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public DbSet<UserCard> Cards { get; set; }
    }
}
