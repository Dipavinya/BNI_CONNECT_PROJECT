using System.ComponentModel.DataAnnotations;

namespace BniConnect.Models
{
    public class WhatsAppSetting
    {
        [Key]
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Access_Token { get; set; }
        public string Instance_Id { get; set; }
        public string ClientId { get; set; }
        public int TemplateId { get; set; }

    }
}
