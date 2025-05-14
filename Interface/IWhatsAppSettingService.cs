using BniConnect.Models;

namespace BniConnect.Interface
{
    public interface IWhatsAppSettingService
    {
        Task<List<WhatsAppSetting>> GetWhatsAppSettingsByClientId(string clientId);
        Task<bool> InsertWhatsappSetting(WhatsAppSetting whatsAppSettings);
        Task<bool> DeleteWhatsApp(int id);
        Task<WhatsAppSetting> GetWhatsAppSettingsById(int Id);
    }
}
