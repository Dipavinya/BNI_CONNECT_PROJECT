using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BniConnect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string client_id { get; set; } = "IDENTITY_PORTAL"; 
        [Required]
        public string user_id { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
