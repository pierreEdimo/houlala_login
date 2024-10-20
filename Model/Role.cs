using Microsoft.AspNetCore.Identity;

namespace user_service.Model;

public class Role : IdentityRole
{
    public virtual UserEntity? User { get; set; }
}
