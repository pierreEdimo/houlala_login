using Microsoft.AspNetCore.Identity;

namespace user_service.Model
{
    public class User : IdentityUser
    {
        public String? Name{get; set;}
        public String? StreetName{get; set;}
        public String? PoBox{get; set;}
        public String? City{get; set;}
        public String? Country{get; set;}
        public String? HouseNumber{get; set;}
    }
}