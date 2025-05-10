using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace LapStore.DAL.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int? id);
        T GetById(int? id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> criteria);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        int Count();
    }
}