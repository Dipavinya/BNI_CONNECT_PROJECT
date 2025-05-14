using System.ComponentModel.DataAnnotations;

namespace BniConnect.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        public string Email {  get; set; }
        public bool IsActive { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public bool Allfeature { get; set; }
    }
}
