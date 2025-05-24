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
    public class GetAllAsyncTest
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            var options = new DbContextOptionsBuilder<LapStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);
            var repo = new GenericRepository<TestEntity>(context);

            await context.TestEntities.AddRangeAsync(
                new TestEntity { Id = 1, Name = "First" },
                new TestEntity { Id = 2, Name = "Second" });
            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

    }
}
