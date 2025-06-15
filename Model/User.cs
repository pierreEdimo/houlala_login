using Microsoft.AspNetCore.Identity;

namespace user_service.Model;

public class UserEntity : IdentityUser
{
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? RoleId { get; init; }
    public int? DeliveryAddressId { get; set; }
    public virtual Role? Role { get; init; }
    public virtual List<Address>? Addresses { get; init; }
}