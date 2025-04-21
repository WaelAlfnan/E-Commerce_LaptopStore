using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LapStore.Test.Helpers;
using LapStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LapStore.Test.RepositoriesTest.GenericRepositoryTest
{
    public class UpdateTest
    {
        [Fact]
        public async Task Update_UpdatesEntity()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);
            var repo = new GenericRepository<TestEntity>(context);

            var entity = new TestEntity { Id = 1, Name = "Before" };
            await context.TestEntities.AddAsync(entity);
            await context.SaveChangesAsync();

            entity.Name = "After";
            repo.Update(entity);
            await context.SaveChangesAsync();

            var updated = await context.TestEntities.FindAsync(1);
            Assert.Equal("After", updated.Name);
        }

    }
}
