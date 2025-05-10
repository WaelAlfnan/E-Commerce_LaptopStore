using LapStore.DAL.Data.Entities;
using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(LapStoreDbContext context) : base(context)
        {
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<bool> IsCategoryNameExistAsync(string categoryName, int? categoryId = null)
        {
            var query = _dbSet.Where(c => c.Name == categoryName);

            if (categoryId.HasValue)
            {
                // Exclude the current category from the check
                query = query.Where(c => c.Id != categoryId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
