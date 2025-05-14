using BniConnect.Models;

namespace BniConnect.Interface
{
    public interface IMailSettings
    {
        Task<List<MailSettings>> GetMailSettingsByClientId(string cliendId);
        Task <int> GetLastUsedMailTemplateByClientId(string cliendId);
        Task<bool> InsertMailSetting(MailSettings mailSettings);
        Task<MailSettings> GetMailSettingsById(int Id);
        Task<bool> DeleteEmail(int id);
    }
}
