using System.ComponentModel.DataAnnotations;

namespace user_service.Dto;

public class LoginDto
{
    [Required] public string? Email { get; set; }
    [Required] public string? PassWord { get; set; }
}