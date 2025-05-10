using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LapStore.DAL.Data.Contexts
{
    // Change to inherit from IdentityDbContext with the correct generic parameters
    public class LapStoreDbContext : IdentityDbContext<User, IdentityRole<int>, int, IdentityUserClaim<int>,
        IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public LapStoreDbContext(DbContextOptions<LapStoreDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies();

                // Disable database creation warnings
                optionsBuilder.ConfigureWarnings(warnings =>
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ManyServiceProvidersCreatedWarning));
            }
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
        // Users are automatically included by IdentityDbContext, but we can still define it
        public DbSet<User> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call base first to set up Identity tables
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for all price and weight columns
            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Weight)
                .HasColumnType("decimal(8,3)");

            // Define primary keys explicitly (except User which is handled by Identity)
            modelBuilder.Entity<Address>().HasKey(a => a.Id);
            modelBuilder.Entity<Cart>().HasKey(c => c.Id);
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<ProductImage>().HasKey(pi => pi.Id);
            // User key is configured by Identity

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
                .HasForeignKey(u => u.AddressId)
                .OnDelete(DeleteBehavior.Restrict);  // Added explicit delete behavior

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
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Indexers
            modelBuilder.Entity<User>().HasIndex(u => new { u.UserName, u.Email, u.PhoneNumber })
                .IsUnique();

            modelBuilder.Entity<User>().HasIndex(u => new { u.FirstName, u.LastName });

            // Customize Identity table names if needed
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        }
    }
}