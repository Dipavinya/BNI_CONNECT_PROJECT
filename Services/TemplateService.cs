using BniConnect.Data;
using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BniConnect.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TemplateService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<TemplateMaster>> GetTemplatesByClient(string clientId)
        {
            try
            {
                var templates = await _context.Templates
                                .Where(t => t.ClientId == clientId || t.Id == 1)
                                .ToListAsync();

                if (templates.Count == 0)
                {
                    var defaultTemplate = await _context.Templates.FirstOrDefaultAsync(t => t.Id == 1);
                    if (defaultTemplate != null)
                    {
                        return new List<TemplateMaster> { defaultTemplate };
                    }
                    else
                    {
                        return new List<TemplateMaster>();
                    }
                }
                return templates;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while getting TemplatesByClientId");
                return null;
            }
        }

        public async Task<bool> InsertTemplate(TemplateMaster template)
        {
            try
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var clientId = user?.Id;

                if (template.Id > 0)
                {
                    var existingTemplate = await _context.Templates.FindAsync(template.Id);
                    if (existingTemplate != null)
                    {
                        existingTemplate.TemplateName = template.TemplateName;
                        existingTemplate.Subject = template.Subject;
                        existingTemplate.Body = template.Body;
                        existingTemplate.ClientId = clientId;

                        _context.Templates.Update(existingTemplate);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var templatedata = new TemplateMaster
                    {
                        ClientId = clientId,
                        TemplateName = template.TemplateName,
                        Subject = template.Subject,
                        Body = template.Body
                    };
                    await _context.Templates.AddAsync(templatedata);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error while inserting Template");
                return false;
            }
        }

        public async Task<List<TemplateMaster>> GetAllTemplates()
        {
            var templates = await _context.Templates.ToListAsync();
            return templates;
        }

        public async Task<bool> DeleteTemplate(int id)
        {
            try
            {
                if (id > 0)
                {
                    var template = await _context.Templates.FindAsync(id);
                    if (template != null)
                    {
                        _context.Templates.Remove(template); 
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error while Deleting Template");
                return false; 
            }
            return false; 
        }

        public async Task<TemplateMaster> GetTemplateById(int id)
        {
            if (id > 0)
            {
                var template = await _context.Templates.FindAsync(id);
                return template; 
            }
            else
            {
                return null; 
            }
        }

    }
}
