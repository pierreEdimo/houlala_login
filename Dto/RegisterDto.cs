using System.ComponentModel.DataAnnotations;

namespace user_service.Dto;

public class RegisterDto
{
    [Required] public string? UserName { get; set; }
    [Required] public string? PassWord { get; set; }
    [Required] public string? Email { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? PhoneNumber { get; set; }

    public string? RoleId { get; set; }
}