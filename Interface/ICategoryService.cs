using BniConnect.Models;

namespace BniConnect.Interface
{
    public interface ICategoryService
    {
        Task<List<CategoryMst>> Getcategorys();
        Task<List<SubCategoryMst>> GetSubCategoryByCatId(int catCode);
        Task<List<SubCategoryMst>> GetSubCategories();
    }
}
