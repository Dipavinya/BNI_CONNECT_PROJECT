using BniConnect.Data;
using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BniConnect.Services
{
    public class MailSettingService : IMailSettings
    {
        private readonly ApplicationDbContext _context;

        public MailSettingService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<MailSettings>> GetMailSettingsByClientId(string clientId)
        {
            return await _context.GmailSettings
                .Where(m => m.ClientId == clientId)
                .OrderByDescending(m => m.Id) 
                .ToListAsync();
        }
        public async Task<int> GetLastUsedMailTemplateByClientId(string cliendId)
        {
            var mailSettings = await GetMailSettingsByClientId(cliendId);
            if (mailSettings.Count == 0)
            {
                return 1;
            }
            return mailSettings.First().TemplateId;
        }

        public async Task<bool> InsertMailSetting(MailSettings mailSettings)
        {

            if (mailSettings.Id > 0)
            {
                var existingMailSetting = await _context.GmailSettings.FindAsync(mailSettings.Id);
                if (existingMailSetting != null)
                {
                    existingMailSetting.UserMail = mailSettings.UserMail;
                    existingMailSetting.GmailAppKey = mailSettings.GmailAppKey;
                    existingMailSetting.ClientId = mailSettings.ClientId;
                    existingMailSetting.TemplateId = mailSettings.TemplateId;
                    existingMailSetting.DisplayName = mailSettings.DisplayName;

                    _context.GmailSettings.Update(existingMailSetting);
                }
            }
            else
            {
                _context.GmailSettings.Add(mailSettings);
            }
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<MailSettings> GetMailSettingsById(int Id)
        {
            if (Id > 0)
            {
                var email = await _context.GmailSettings.FindAsync(Id);
                return email;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteEmail(int id)
        {
            try
            {
                if (id > 0)
                {
                    var email = await _context.GmailSettings.FindAsync(id);
                    if (email != null)
                    {
                        _context.GmailSettings.Remove(email);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while Deleting Email configuration");
                return false;
            }
            return false;
        }
    }
}
