using Store.DAL.Data.Contexts;
using Store.DAL.Repositories;

namespace Store.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GenericRepository<T>() where T : class;
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        Task<int> CompleteAsync();
        
        // Transaction methods
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        StoreDbContext Context {  get; }
    }
}
