using System.ComponentModel.DataAnnotations;

namespace user_service.Model;

public class Address
{
    public int Id { get; set; }

    [StringLength(255)] public string? Street { get; set; }

    [StringLength(255)] public string? City { get; set; }

    [StringLength(255)] public string? Country { get; set; }

    [StringLength(255)] public string? PoBox { get; set; }

    [StringLength(255)] public int? HouseNumber { get; set; }

    public string? UserId { get; set; }

    public UserEntity? User { get; set; }
}