using LapStore.Test.Helpers;
using LapStore.DAL;
using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LapStore.Test.UnitOfWorkTest
{
    public class CompleteAsyncTest
    {
        [Fact]
        public async Task CompleteAsync_SavesChanges()
        { //Arrange
            // Arrange: Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<LapStoreDbContext>()
                .UseInMemoryDatabase(databaseName: "SaveTestDB")
                .Options;

            // Create a new DbContext instance
            using var context = new LapStoreDbContext(options);
            var unitOfWork = new UnitOfWork(context);

            // Ensure the 'TestEntity' table is properly initialized and tracked by the context
            var repo = unitOfWork.GenericRepository<TestEntity>();

            // Act: Add a new entity
            var testEntity = new TestEntity { Id = 1, Name = "Saved" };
            await repo.AddAsync(testEntity);

            // Simulate saving changes to the in-memory database
            var result = await unitOfWork.CompleteAsync();

            // Act: Retrieve the entity from the context to check if it was saved
            var savedEntity = await context.Set<TestEntity>().FindAsync(testEntity.Id);

            // Assert: Verify that the changes were saved
            Assert.Equal(1, result);  // 1 record saved
            Assert.NotNull(savedEntity); // Ensure the entity was saved
            Assert.Equal(testEntity.Name, savedEntity.Name); // Check that the name is saved correctly
        }
    }
} 