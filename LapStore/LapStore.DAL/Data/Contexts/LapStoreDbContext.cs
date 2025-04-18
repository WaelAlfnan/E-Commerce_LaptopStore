﻿using LapStore.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapStore.DAL.Data.Contexts
{
    public class LapStoreDbContext : DbContext
    {
        public LapStoreDbContext(DbContextOptions<LapStoreDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<Address> addresses { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<CartItem> cartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductImage> productImages { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<User> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary keys explicitly
            modelBuilder.Entity<Address>().HasKey(a => a.Id);
            modelBuilder.Entity<Cart>().HasKey(c => c.Id);
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<ProductImage>().HasKey(pi => pi.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);


            // Composite Key Configurations
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId });

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.ProductId });

            modelBuilder.Entity<Review>()
                .HasKey(r => new { r.UserId, r.ProductId });


            // Example with Cart and User (One to One)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.user)
                .WithOne(u => u.cart)
                .HasForeignKey<Cart>(c => c.UserId);

            // Example with Address and User (One to One)
            modelBuilder.Entity<User>()
            .HasOne(u => u.address)
            .WithMany(a => a.users)
            .HasForeignKey(u => u.AddressId);

            // Example with Category and ParentCategory (Self-Referencing)
            modelBuilder.Entity<Category>()
                .HasOne(c => c.parentCategory)
                .WithMany(c => c.childCategories)
                .HasForeignKey(c => c.ParentCategoryId);

            // Example with Product and Category (Many to One)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.category)
                .WithMany(c => c.products)
                .HasForeignKey(p => p.CategoryId);

            // Example with Order and User (Many to One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.user)
                .WithMany(u => u.orders)
                .HasForeignKey(o => o.UserId);

            // Example with ProductImage and Product (Many to One)
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.product)
                .WithMany(p => p.productImages)
                .HasForeignKey(pi => pi.ProductId);

            // Example with CartItem and Cart (Many to One)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.cart)
                .WithMany(c => c.cartItems)
                .HasForeignKey(ci => ci.CartId);

            // Example with CartItem and Product (Many to One)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.product)
                .WithMany(p => p.cartItems)
                .HasForeignKey(ci => ci.ProductId);

            // Example with OrderItem and Order (Many to One)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.order)
                .WithMany(o => o.orderItems)
                .HasForeignKey(oi => oi.OrderId);

            // Example with OrderItem and Product (Many to One)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.product)
                .WithMany(p => p.orderItems)
                .HasForeignKey(oi => oi.ProductId);

            // Example with Review and User (Many to One)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.user)
                .WithMany(u => u.userReviews)
                .HasForeignKey(r => r.UserId);


            // Enforce 'Restrict' delete behavior for all foreign key relationships
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict; // Prevent deletion if related entities exist
            }
            // Indexers

            modelBuilder.Entity<User>().HasIndex(u => new { u.UserName, u.Email, u.PhoneNumber })
                .IsUnique();

            modelBuilder.Entity<User>().HasIndex(u => new { u.FirstName, u.LastName });

            //modelBuilder.Entity<User>().ToTable("users", u => u.ExcludeFromMigrations());

            base.OnModelCreating(modelBuilder);
        }



    }
}
