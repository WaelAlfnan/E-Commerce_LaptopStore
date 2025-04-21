using LapStore.Test.Helpers;
using LapStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LapStore.Test.Helpers;


namespace LapStore.Test.RepositoriesTest.GenericRepositoryTest
{
    public class CountTest
    {
        [Fact]
        public async Task Count_ReturnsCorrectCount()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);
            var repo = new GenericRepository<TestEntity>(context);

            await context.TestEntities.AddRangeAsync(
                new TestEntity { Id = 1, Name = "One" },
                new TestEntity { Id = 2, Name = "Two" });
            await context.SaveChangesAsync();

            var count = repo.Count();

            Assert.Equal(2, count);
        }

    }
}
