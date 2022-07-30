using System.ComponentModel.DataAnnotations;

namespace user_service.Dto
{
    public class RegisterDto
    {
        [Required]
        public String? UserName { get; set; }

        [Required]
        public String? passWord { get; set; }

        [Required]
        public String? Email { get; set; }
    }

}