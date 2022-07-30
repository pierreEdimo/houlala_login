using System.ComponentModel.DataAnnotations;

namespace user_service.Dto
{
    public class LoginDto
    {
        [Required]
        public String? Email{ get; set; }

        [Required]
        public String? PassWortd { get; set; }
    }
}