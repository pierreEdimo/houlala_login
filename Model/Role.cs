using Microsoft.AspNetCore.Identity;

namespace user_service.Model;

public class Role : IdentityRole
{
    public virtual User? User { get; set; }
}
