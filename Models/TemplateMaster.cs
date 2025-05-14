using System.ComponentModel.DataAnnotations;

namespace BniConnect.Models
{
    public class TemplateMaster
    {
        [Key]
        public int Id { get; set; }
        public string? ClientId { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
