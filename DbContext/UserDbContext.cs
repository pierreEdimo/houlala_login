using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; 
using user_service.Model; 

namespace user_service.DbContext
{
    public class UserDbContext : IdentityDbContext<User>
    {
        public UserDbContext() {}
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
    }
}