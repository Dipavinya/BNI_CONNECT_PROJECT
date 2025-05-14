using BniConnect.Data;
using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BniConnect.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<bool> InsertSubscription(Subscription subscription)
        {
            try
            {
                // Check if a subscription already exists with the provided email
                var checkSubscription = await GetSubscriptionByEmail(subscription.Email);

                // If subscription Id is greater than 0, update the existing subscription
                if (subscription.Id > 0)
                {
                    var existingSub = await _context.Subscription.FindAsync(subscription.Id);

                    if (existingSub != null)
                    {
                        existingSub.Email = subscription.Email;
                        existingSub.SubscriptionDate = DateTime.UtcNow;
                        existingSub.Allfeature = subscription.Allfeature;
                        _context.Subscription.Update(existingSub);
                    }
                    else
                    {
                        Log.Warning($"Subscription with Id {subscription.Id} not found.");
                    }
                }
                else
                {
                    if (checkSubscription == null)
                    {
                        var sub = new Subscription
                        {
                            SubscriptionDate = DateTime.UtcNow,
                            IsActive = true,
                            Allfeature = subscription.Allfeature,
                            Email = subscription.Email
                        };
                        await _context.Subscription.AddAsync(sub);
                    }
                    else
                    {
                        Log.Information("Subscription with this email already exists.");
                        return false; // Prevent adding a duplicate subscription
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log detailed exception
                Log.Error(ex, "Error while inserting or updating SubscriptionByEmail");

                if (ex.InnerException != null)
                {
                    Log.Error(ex.InnerException, "Inner exception details");
                }

                return false;
            }
        }


        public async Task<Subscription> GetSubscriptionByEmail(string email)
        {
            return await _context.Subscription.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Subscription> GetSubscriptionById(int id)
        {
            return await _context.Subscription.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Subscription>> GetSubscriptions()
        {
            return await _context.Subscription.ToListAsync();
        }

        public async Task<bool> DeleteSubscription(int id)
        {
            var subscription = await _context.Subscription.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscription.Remove(subscription);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
