using BniConnect.Data;
using BniConnect.Interface;
using BniConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace BniConnect.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryMst>> Getcategorys()
        {
            var category = await _context.CategoryMst.ToListAsync();
            return category;
        }

        public async Task<List<SubCategoryMst>> GetSubCategories()
        {
            var subCategory = await _context.SubCategoryMst.ToListAsync();
            return subCategory;
        }

        public async Task<List<SubCategoryMst>> GetSubCategoryByCatId(int catCode)
        {
            if (catCode > 0)
            {
                var categories = await _context.SubCategoryMst.Where(c => c.CatCode.ToString() == catCode.ToString()).ToListAsync();
                return categories;
            }
            else
            {
                return new List<SubCategoryMst>();
            }
        }
    }
}
