using System.ComponentModel.DataAnnotations;

namespace user_service.Dto
{
    public class RegisterDto
    {
        [Required]
        public String? UserName { get; set; }
        [Required]
        public String? PassWord { get; set; }
        [Required]
        public String? Email { get; set; }
        public String? Name { get; set; }
        public String? StreetName { get; set; }
        public String? PoBox { get; set; }
        public String? City { get; set; }
        public String? Country { get; set; }
        public String? HouseNumber { get; set; }
        public String? PhoneNumber{get; set;}
    }

}