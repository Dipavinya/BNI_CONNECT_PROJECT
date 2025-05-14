namespace BniConnect.Models
{
    public class EmailSentHistoryDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public DateTime SentDate { get; set; }
        public string UserEmail { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public bool IsWhasAppSent { get; set; }
        public bool IsConnectionSent { get; set; }

    }
}
