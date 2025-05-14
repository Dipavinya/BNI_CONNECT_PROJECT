using BniConnect.Models;

namespace BniConnect.Interface
{
    public interface ITemplateService
    {
        Task<List<TemplateMaster>> GetTemplatesByClient(string clientId);
        Task<bool> InsertTemplate(TemplateMaster template);
        Task<List<TemplateMaster>> GetAllTemplates();
        Task<bool> DeleteTemplate(int id);
        Task<TemplateMaster> GetTemplateById(int id);
    }
}
