using System.ComponentModel.DataAnnotations;

namespace BniConnect.Models
{
    public class MailSettings
    {
        [Key]
        public int Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Required(ErrorMessage = "Email is required.")]
        public string UserMail {  get; set; }
        public string GmailAppKey { get; set; }
        public string ClientId { get; set; }
        public string DisplayName {  get; set; }
        public int TemplateId { get; set; }
    }
}
