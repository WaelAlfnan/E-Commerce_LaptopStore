using LapStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LapStore.Test.Helpers;
using Xunit;
using LapStore.DAL.Data.Contexts;

namespace LapStore.Test.RepositoriesTest.GenericRepositoryTest
{
    public class AddAsyncTest
    {
        [Fact]
        public async Task AddAsync_AddsEntitySuccessfully()
        {
            var options = new DbContextOptionsBuilder<LapStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);
            var repo = new GenericRepository<TestEntity>(context);

            var entity = new TestEntity { Id = 1, Name = "Test" };
            await repo.AddAsync(entity);
            await context.SaveChangesAsync();

            Assert.Equal(1, context.TestEntities.Count());
        }
    }
}
