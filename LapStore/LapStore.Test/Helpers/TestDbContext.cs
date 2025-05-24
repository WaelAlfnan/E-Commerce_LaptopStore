using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.Test.Helpers
{
    public class TestDbContext : LapStoreDbContext
    {
        public TestDbContext(DbContextOptions<LapStoreDbContext> options) : base(options) 
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; }
    }
} 