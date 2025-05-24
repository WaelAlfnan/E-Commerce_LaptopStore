using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LapStore.Test.Helpers;
using LapStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using LapStore.DAL.Data.Contexts;

namespace LapStore.Test.RepositoriesTest.GenericRepositoryTest
{
    public class GetByIdAsyncTest
    {
        [Fact]
        public async Task GetByIdAsync_ReturnsEntityById()
        {
            var options = new DbContextOptionsBuilder<LapStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);
            var repo = new GenericRepository<TestEntity>(context);

            var entity = new TestEntity { Id = 1, Name = "Test" };
            await context.TestEntities.AddAsync(entity);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
        }


    }
}
