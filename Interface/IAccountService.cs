using BniConnect.Models;

namespace BniConnect.Interface
{
    public interface IAccountService
    {
        Task<bool> InsertUserProfile(UserProfile userProfile);
        Task<UserProfile> GetUserById(string userId);
        Task<UserProfile> GetEmailByUserId(string userId);        
        Task<UserProfile> GetNumberByUserId(string userId);
        Task<UserProfile> GetUserByPhone(string phoneNumber);
        Task<bool> SaveConnectinHistory(UserProfile model, bool status,string clientId);
        Task<List<ConnectionSentHistoryDto>> GetConnectionHistoryByClientId(string clinetId);
        Task<bool> DeleteConnection(string[] ids);
        Task<bool> LoginUser(string UserName, string Password);
    }
}
