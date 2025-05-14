namespace BniConnect.Models
{
    public class ConnectionSentHistory
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Industry { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsSuccess { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }

    }
}
    