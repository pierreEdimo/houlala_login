using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using user_service.Model;

namespace user_service.DbContext;

public class UserDbContext : IdentityDbContext<UserEntity, Role, string, IdentityUserClaim<string>,
    IdentityUserRole<string>, IdentityUserLogin<string>,
    IdentityRoleClaim<string>, IdentityUserToken<string>>
{
    public UserDbContext()
    {
    }

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<Address>? Addresses { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>().HasOne(x => x.Role).WithOne(x => x.User)
            .HasForeignKey<UserEntity>(x => x.RoleId);

        modelBuilder.Entity<Address>().HasOne(x => x.User).WithMany(x => x.Addresses).HasForeignKey(x => x.UserId);
    }
}