using Microsoft.EntityFrameworkCore;
using Order_Management_System.Models;

namespace Order_Management_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Decimal precision ──────────────────────────
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // ✅ Dùng Price thay vì UnitPrice
            modelBuilder.Entity<OrderItem>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            // ✅ Fix lỗi CreatedAt với MySQL strict mode
            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("NOW(6)");

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("NOW(6)");

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("NOW(6)");

            modelBuilder.Entity<Order>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("NOW(6)");

            // ── Seed Categories ────────────────────────────
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Nước khoáng", Description = "Nước khoáng thiên nhiên", CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = 2, Name = "Nước tinh khiết", Description = "Nước tinh khiết đóng chai", CreatedAt = new DateTime(2024, 1, 1) },
                new Category { Id = 3, Name = "Nước có gas", Description = "Nước khoáng có gas", CreatedAt = new DateTime(2024, 1, 1) }
            );

            // ── Seed Admin user ────────────────────────────
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "Administrator",
                Email = "admin@mineralwater.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                // ✅ Dùng Role.Admin đúng enum của project
                Role = Role.Admin,
                CreatedAt = new DateTime(2024, 1, 1)
            });

            // ── Seed Products ──────────────────────────────
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "La Vie 500ml", Description = "Nước khoáng 500ml", Price = 6000, StockQuantity = 500, CategoryId = 1, CreatedAt = new DateTime(2024, 1, 1) },
                new Product { Id = 2, Name = "La Vie 1.5L", Description = "Nước khoáng 1.5L", Price = 12000, StockQuantity = 300, CategoryId = 1, CreatedAt = new DateTime(2024, 1, 1) },
                new Product { Id = 3, Name = "La Vie 19L", Description = "Bình nước 19L", Price = 65000, StockQuantity = 8, LowStockThreshold = 10, CategoryId = 1, CreatedAt = new DateTime(2024, 1, 1) },
                new Product { Id = 4, Name = "Tinh khiết 350ml", Description = "Nước tinh khiết", Price = 4000, StockQuantity = 1000, CategoryId = 2, CreatedAt = new DateTime(2024, 1, 1) },
                new Product { Id = 5, Name = "Sparkling 330ml", Description = "Nước khoáng có gas", Price = 15000, StockQuantity = 200, CategoryId = 3, CreatedAt = new DateTime(2024, 1, 1) }
            );
        }

        // ✅ Tự động set CreatedAt khi tạo mới
        public override int SaveChanges()
        {
            SetTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                try
                {
                    var prop = entry.Property("CreatedAt");
                    if (prop != null)
                        prop.CurrentValue = DateTime.UtcNow;
                }
                catch { /* bỏ qua nếu entity không có CreatedAt */ }
            }
        }
    }
}