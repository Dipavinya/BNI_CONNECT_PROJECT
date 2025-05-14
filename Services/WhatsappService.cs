using BniConnect.Interface;
using BniConnect.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using Serilog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using BniConnect.Data;
using Microsoft.EntityFrameworkCore;

namespace BniConnect.Services
{
    public class WhatsappService : IWhatsappService
    {
        private readonly ApplicationDbContext _context;

        public WhatsappService( ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<bool> SaveWhatsAppHistory(UserProfile model,bool status,string message,string clientId)
        {
            try
            {
                if (model != null)
                {
                    var history = new WhatsAppSentHistory
                    {
                        DisplayName = model.DisplayName,
                        Industry = model.Industry,
                        PhoneNumber = model.PhoneNumber,
                        Message = message,
                        SentDate = DateTime.Now,
                        IsSuccess  = status,
                        ClientId = clientId,
                        UserId = model.UserId
                    };
                    _context.WhatsAppSentHistory.Add(history);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error while saving Whatsapp History");
                return false;
            }
            return false;
        }

        public async Task<List<WhatsAppSentHistoryDto>> GetWhatsAppHistoryByClientId(string clientId)
        {
            try
            {
                var WhatsAppHistory = await _context.WhatsAppSentHistory
                                                .Where(x => x.ClientId == clientId)
                                                .ToListAsync();

                List<WhatsAppSentHistoryDto> result = new List<WhatsAppSentHistoryDto>();

                foreach (var record in WhatsAppHistory)
                {
                    var emailHistory = await _context.EmailSentHistory
                                                     .Where(x => x.UserId == record.UserId)
                                                     .ToListAsync();

                    var conhistory = await _context.ConnectionSentHistory
                                                      .Where(x => x.UserId == record.UserId)
                                                      .ToListAsync();

                    bool isMailSent = emailHistory.Any();
                    bool isConSent = conhistory.Any();

                    var HistoryDto = new WhatsAppSentHistoryDto
                    {
                        Id = record.Id,
                        DisplayName = record.DisplayName,
                        Industry = record.Industry,
                        PhoneNumber = record.PhoneNumber,
                        SentDate = record.SentDate,
                        IsSuccess = record.IsSuccess,
                        ClientId = record.ClientId,
                        UserId = record.UserId,
                        IsMailSent = isMailSent,
                        IsConnectionSent = isConSent
                    };

                    result.Add(HistoryDto);
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while retrieving Whatsapp History");
                return null;
            }
        }

        public async Task<string> GetWhatsAppMsgById(int id)
        {
            var Message = await _context.WhatsAppSentHistory.FindAsync(id);
            return Message.Message;
        }

        public async Task<bool> DeleteWhatsAppHistory(string[] ids)
        {
            try
            {
                if (ids != null && ids.Length > 0)
                {
                    foreach (var id in ids)
                    {
                        var WhatsAppHistory = await GetWhatsAppByUserID(id);
                        if (WhatsAppHistory != null)
                        {
                            _context.WhatsAppSentHistory.Remove(WhatsAppHistory);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while deleting Email history");
                return false;
            }

            return false;
        }

        public async Task<WhatsAppSentHistory> GetWhatsAppByUserID(string userId)
        {
            return await _context.WhatsAppSentHistory.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        }
    }
}
