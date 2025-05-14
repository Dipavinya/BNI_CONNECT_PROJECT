using BniConnect.Data;
using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BniConnect.Services
{
    public class WhatsAppSettingService : IWhatsAppSettingService
    {
        private readonly ApplicationDbContext _context;

        public WhatsAppSettingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WhatsAppSetting>> GetWhatsAppSettingsByClientId(string clientId)
        {
            return await _context.WhatsAppSetting
                .Where(m => m.ClientId == clientId)
                .OrderByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<bool> InsertWhatsappSetting(WhatsAppSetting whatsAppSettings)
        {
            try
            {
                if (whatsAppSettings.Id > 0)
                {
                    var existingwhatsAppSetting = await _context.WhatsAppSetting.FindAsync(whatsAppSettings.Id);
                    if (existingwhatsAppSetting != null)
                    {
                        existingwhatsAppSetting.Access_Token = whatsAppSettings.Access_Token;
                        existingwhatsAppSetting.Instance_Id = whatsAppSettings.Instance_Id;
                        existingwhatsAppSetting.ClientId = whatsAppSettings.ClientId;
                        existingwhatsAppSetting.TemplateId = whatsAppSettings.TemplateId;
                        existingwhatsAppSetting.DisplayName = whatsAppSettings.DisplayName;

                        _context.WhatsAppSetting.Update(existingwhatsAppSetting);
                    }
                }
                else
                {
                    _context.WhatsAppSetting.Add(whatsAppSettings);
                }
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while inserting WhatsappSetting");
                return false;
            }
        }

        public async Task<bool> DeleteWhatsApp(int id)
        {
            try
            {
                if (id > 0)
                {
                    var whatsApp = await _context.WhatsAppSetting.FindAsync(id);
                    if (whatsApp != null)
                    {
                        _context.WhatsAppSetting.Remove(whatsApp);
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

        public async Task<WhatsAppSetting> GetWhatsAppSettingsById(int Id)
        {
            if (Id > 0)
            {
                var whatsApp = await _context.WhatsAppSetting.FindAsync(Id);
                return whatsApp;
            }
            else
            {
                return null;
            }
        }
    }
}
