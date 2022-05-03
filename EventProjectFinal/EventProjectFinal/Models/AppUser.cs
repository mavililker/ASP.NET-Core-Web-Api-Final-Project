using Microsoft.AspNetCore.Identity;

namespace EventProjectFinal.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }

        public string SurName { get; set; }

        public string Role { get; set; }
    }
}
