using BniConnect.Models;

namespace BniConnect.Interface
{
    public interface ISubscriptionService
    {
        Task<bool> InsertSubscription(Subscription subscription);
        Task<Subscription> GetSubscriptionByEmail(string email);
        Task<List<Subscription>> GetSubscriptions();
        Task<bool> DeleteSubscription(int id);
        Task<Subscription> GetSubscriptionById(int id);
    }
}
