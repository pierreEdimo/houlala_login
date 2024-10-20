using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using user_service.Model;
using Microsoft.AspNetCore.Identity;

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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>().HasOne(x => x.Role).WithOne(x => x.User).HasForeignKey<UserEntity>(x => x.RoleId);
    }
}
