using BniConnect.Data;
using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Serilog;


namespace BniConnect.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly ApplicationDbContext _context;

        public EmailService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<bool> SendEmail(string userEmail, string userPassword, string DisplayName,string to, string subject, string body, IFormFile attachment)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(DisplayName, userEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Headers.Add("X-Gmail-Labels", "TestBNI");
                
                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                if (attachment != null && attachment.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await attachment.CopyToAsync(memoryStream);
                        bodyBuilder.Attachments.Add(attachment.FileName, memoryStream.ToArray(), ContentType.Parse(attachment.ContentType));
                    }
                }
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(userEmail, userPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error while sending email");
                return false;
            }
        }
        
        public async Task<bool> SaveEmailHistory(string displayName, string message, string recipientEmail, string userEmail, string subject, bool status,string clientId,string recipientUserId)
        {
            try
            {
                var emailHistory = new EmailSentHistory
                {
                    UserEmail = userEmail,
                    Subject = subject,
                    DisplayName = displayName,
                    RecipientEmail = recipientEmail,
                    SentDate = DateTime.UtcNow,
                    IsSuccess = status,
                    Message = message,
                    ClientId = clientId,
                    UserId = recipientUserId
                };

                _context.EmailSentHistory.Add(emailHistory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Issue in saving email history");
                return false;
            }
        }

        public async Task<List<EmailSentHistoryDto>> GetEmailHistoryByClientId(string clientId)
        {
            try
            {
                var emailHistory = await _context.EmailSentHistory
                                                .Where(x => x.ClientId == clientId)
                                                .ToListAsync();

                List<EmailSentHistoryDto> result = new List<EmailSentHistoryDto>();

                foreach (var record in emailHistory)
                {
                    var conHistory = await _context.ConnectionSentHistory
                                                     .Where(x => x.UserId == record.UserId)
                                                     .ToListAsync();

                    var whatshistory = await _context.WhatsAppSentHistory
                                                      .Where(x => x.UserId == record.UserId)
                                                      .ToListAsync();

                    bool isConSent = conHistory.Any();
                    bool isWhatsAppSent = whatshistory.Any();

                    var HistoryDto = new EmailSentHistoryDto
                    {
                        Id = record.Id,
                        DisplayName = record.DisplayName,
                        SentDate = record.SentDate,
                        RecipientEmail = record.RecipientEmail,
                        Subject = record.Subject,
                        UserEmail = record.UserEmail,
                        IsSuccess = record.IsSuccess,
                        ClientId = record.ClientId,
                        UserId = record.UserId,
                        IsConnectionSent = isConSent,
                        IsWhasAppSent = isWhatsAppSent
                    };

                    result.Add(HistoryDto);
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while retrieving Connection History");
                return null;
            }
        }

        public async Task<string> GetEmailById(int id)
        {
            var history = await _context.EmailSentHistory.FindAsync(id);
            return history.Message;
        }

        public async Task<bool> DeleteEmailsHistory(string[] ids)
        {
            try
            {
                if (ids != null && ids.Length > 0)
                {
                    foreach (var id in ids)
                    {
                        var emailHistory = await GetEmailByUserID(id);
                        if (emailHistory != null)
                        {
                            _context.EmailSentHistory.Remove(emailHistory);
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

        public async Task<EmailSentHistory> GetEmailByUserID(string userId)
        {
            return await _context.EmailSentHistory.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<EmailSentHistory> GetEmailByReceipient(string recEmail)
        {
            return await _context.EmailSentHistory.Where(x => x.RecipientEmail == recEmail).FirstOrDefaultAsync();

        }
    }
}
