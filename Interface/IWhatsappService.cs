using BniConnect.Models;

namespace BniConnect.Interface
{
    public interface IWhatsappService
    {
        Task<bool> SaveWhatsAppHistory(UserProfile model, bool status, string message, string clientId);
        Task<List<WhatsAppSentHistoryDto>> GetWhatsAppHistoryByClientId(string clientId);
        Task<string> GetWhatsAppMsgById(int id);
        Task<bool> DeleteWhatsAppHistory(string[] ids);
    }
}
