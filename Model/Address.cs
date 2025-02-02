using System.ComponentModel.DataAnnotations;

namespace user_service.Model;

public class Address
{
    public int Id { get; set; }

    [StringLength(255)] public string? Street { get; init; }
    [StringLength(255)] public string? City { get; init; }
    [StringLength(255)] public string? Country { get; init; }
    [StringLength(255)] public string? PoBox { get; init; }
    [StringLength(255)] public string? HouseNumber { get; init; }
    public string? UserId { get; set; }
    [StringLength(255)] public string? LastName { get; init; }
    [StringLength(255)] public string? FirstName { get; init; }
    public virtual UserEntity? User { get; init; }
}