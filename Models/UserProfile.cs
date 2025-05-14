using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BniConnect.Models
{
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Industry { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        [NotMapped]
        public List<string> SocialNetworkLinks { get; set; }

        [NotMapped] // Use [NotMapped] if you don’t want this property stored in the database.
        public string? PreferredContactNumber { get; set; }
        public string ClientId { get; set; }
    }
}
