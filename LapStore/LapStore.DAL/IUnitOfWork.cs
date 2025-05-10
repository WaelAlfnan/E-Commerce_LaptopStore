using LapStore.DAL.Data.Contexts;
using LapStore.DAL.Repositories;

namespace LapStore.DAL
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
        LapStoreDbContext Context {  get; }
    }
}
