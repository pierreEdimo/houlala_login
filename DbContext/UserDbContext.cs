using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; 
using user_service.Model; 
using Microsoft.AspNetCore.Identity;

namespace user_service.DbContext
{
    public class UserDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>,
    IdentityUserRole<string>, IdentityUserLogin<string>,
    IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public UserDbContext() {}
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
    }
}