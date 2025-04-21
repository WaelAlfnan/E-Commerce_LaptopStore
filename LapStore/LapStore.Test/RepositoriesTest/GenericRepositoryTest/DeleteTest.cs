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
    public class DeleteTest
    {
        [Fact]
        public async Task Delete_RemovesEntity()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);
            var repo = new GenericRepository<TestEntity>(context);

            var entity = new TestEntity { Id = 1, Name = "ToDelete" };
            await context.TestEntities.AddAsync(entity);
            await context.SaveChangesAsync();

            repo.Delete(entity);
            await context.SaveChangesAsync();

            var deleted = await context.TestEntities.FindAsync(1);
            Assert.Null(deleted);
        }

    }
}
