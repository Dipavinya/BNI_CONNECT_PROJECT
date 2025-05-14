namespace BniConnect.Models
{
    public class WhatsAppSentHistoryDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Industry { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsSuccess { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public bool IsMailSent { get; set; }
        public bool IsConnectionSent { get; set; }
    }
}
