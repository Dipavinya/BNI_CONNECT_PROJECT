using BniConnect.Models;

namespace BniConnect.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string userEmail, string userPassword, string DisplayName, string to, string subject, string body, IFormFile attachment);
        Task<bool> SaveEmailHistory(string displayName, string message, string recipientEmail, string userEmail, string subject, bool status, string clientId, string recipientUserId);
        Task<List<EmailSentHistoryDto>> GetEmailHistoryByClientId(string clientId);
        Task<bool> DeleteEmailsHistory(string[] ids);
        Task<string> GetEmailById(int id);
        Task<EmailSentHistory> GetEmailByReceipient(string recEmail);
    }

}
