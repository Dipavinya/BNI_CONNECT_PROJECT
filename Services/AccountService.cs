using BniConnect.Data;
using BniConnect.Interface;
using BniConnect.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BniConnect.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountService(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserProfile> GetEmailByUserId(string userId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = "SELECT * FROM UserProfiles WHERE UserId = @UserId";
            var userProfile = await connection.QueryFirstOrDefaultAsync<UserProfile>(sql, new { UserId = userId });


            //var userProfile = await _context.UserProfiles
            //.FirstOrDefaultAsync(up => up.UserId == userId);
            return userProfile;
        }

        public async Task<UserProfile> GetNumberByUserId(string userId)
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProfile == null)
            {
                return null; // Return null if the user profile is not found.
            }

            userProfile.PreferredContactNumber = !string.IsNullOrEmpty(userProfile.MobileNumber)
                ? userProfile.MobileNumber
                : userProfile.PhoneNumber;

            return userProfile;
        }

        public async Task<UserProfile> GetUserById(string userId)
        {

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = "SELECT * FROM UserProfiles WHERE UserId = @UserId";
            return await connection.QueryFirstOrDefaultAsync<UserProfile>(sql, new { UserId = userId });
            //return await _context.UserProfiles.FindAsync(userId);
        }

        public async Task<UserProfile> GetUserByPhone(string phoneNumber)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }
        public async Task<Member> GetMemberProfileById(string userId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = "SELECT * FROM members_profiles WHERE user_id = @UserId";
            return await connection.QueryFirstOrDefaultAsync<Member>(sql, new { UserId = userId });
        }
        public async Task<bool> InsertUserProfile(UserProfile userProfile)
        {
            try
            {
                if (userProfile != null)   
                {
                    var user = await GetUserById(userProfile.UserId);
                    if (user == null)
                    {
                        await _context.UserProfiles.AddAsync(userProfile);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) {
                Log.Error(ex,"Error while inserting userprofile");
                return false;
            }
        }        

        public async Task<bool> SaveConnectinHistory(UserProfile model,bool status, string clientId)
        {
            try
            {
                if (model != null)
                {
                    var histroy = new ConnectionSentHistory
                    {
                        DisplayName = model.DisplayName,
                        Industry = model.Industry,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        SentDate = DateTime.Now,
                        IsSuccess = status,
                        ClientId = clientId,
                        UserId = model.UserId
                    };
                    _context.ConnectionSentHistory.Add(histroy);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex) 
            {
                Log.Error(ex,"Error While saving ConnectionHistory");
                return false;
            }
            return false;
        }

        public async Task<List<ConnectionSentHistoryDto>> GetConnectionHistoryByClientId(string clientId)
        {
            try
            {
                var conHistory = await _context.ConnectionSentHistory
                                                .Where(x => x.ClientId == clientId)
                                                .ToListAsync();

                List<ConnectionSentHistoryDto> result = new List<ConnectionSentHistoryDto>();

                foreach (var record in conHistory)
                {
                    var emailHistory = await _context.EmailSentHistory
                                                     .Where(x => x.UserId == record.UserId)
                                                     .ToListAsync();

                    var whatshistory = await _context.WhatsAppSentHistory
                                                      .Where(x => x.UserId == record.UserId) 
                                                      .ToListAsync();

                    bool isMailSent = emailHistory.Any();
                    bool isWhatsAppSent = whatshistory.Any();

                    var connectionHistoryDto = new ConnectionSentHistoryDto
                    {
                        Id = record.Id,
                        DisplayName = record.DisplayName, 
                        Industry = record.Industry, 
                        PhoneNumber = record.PhoneNumber, 
                        Email = record.Email, 
                        SentDate = record.SentDate, 
                        IsSuccess = record.IsSuccess, 
                        ClientId = record.ClientId,
                        UserId = record.UserId,
                        IsMailSent = isMailSent,
                        IsWhasAppSent = isWhatsAppSent
                    };

                    result.Add(connectionHistoryDto);
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while retrieving Connection History");
                return null;
            }
        }


        public async Task<bool> DeleteConnection(string[] ids)
        {
            try
            {
                if (ids != null && ids.Any())
                {
                    foreach (var id in ids)
                    {
                        var connection = await GetConnectionByUserID(id);
                        if (connection != null)
                        {
                            _context.ConnectionSentHistory.Remove(connection);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while deleting Connection");
                return false;
            }

            return false;
        }

        public async Task<ConnectionSentHistory> GetConnectionByUserID(string userId)
        {
            return await _context.ConnectionSentHistory.Where(x=> x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<bool> LoginUser(string UserName, string Password)
        {
            var user = await _context.AdminLogin.FirstOrDefaultAsync(x => x.UserName == UserName && x.Pasword == Password);
            return user != null;
        }
    }
}
