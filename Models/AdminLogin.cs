namespace BniConnect.Models
{
    public class AdminLogin
    {
        public int AdminLoginId { get; set; }
        public string? UserName { get; set; }
        public string? Pasword { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
