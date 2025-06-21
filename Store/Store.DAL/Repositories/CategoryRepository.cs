using Store.DAL.Data.Entities;
using Store.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DAL.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly StoreDbContext _context;

        public CategoryRepository(StoreDbContext context) : base(context)
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
