using BniConnect.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BniConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<CountryMst> CountryMst { get; set; }
        public DbSet<TemplateMaster> Templates { get; set; }
        public DbSet<MailSettings> GmailSettings { get; set; }
        public DbSet<Subscription> Subscription { get; set; }
        public DbSet<EmailSentHistory> EmailSentHistory { get; set; }
        public DbSet<WhatsAppSentHistory> WhatsAppSentHistory { get; set; }
        public DbSet<ConnectionSentHistory> ConnectionSentHistory { get; set; }
        public DbSet<WhatsAppSetting> WhatsAppSetting { get; set; }
        public DbSet<CategoryMst> CategoryMst { get; set; }
        public DbSet<SubCategoryMst> SubCategoryMst { get; set; }
        public DbSet<AdminLogin> AdminLogin { get; set; }
    }
}
