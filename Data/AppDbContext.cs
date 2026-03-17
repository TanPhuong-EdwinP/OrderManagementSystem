using Microsoft.EntityFrameworkCore;
using Order_Management_System.Models;

namespace Order_Management_System.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CartItem> CartItems { get; set; }  // ✅ thêm

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Decimal precision ──────────────────────────────
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(i => i.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        // ── Default timestamps ─────────────────────────────
        modelBuilder.Entity<Category>()
            .Property(c => c.CreatedAt).HasDefaultValueSql("NOW(6)");
        modelBuilder.Entity<Product>()
            .Property(p => p.CreatedAt).HasDefaultValueSql("NOW(6)");
        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt).HasDefaultValueSql("NOW(6)");
        modelBuilder.Entity<Order>()
            .Property(o => o.CreatedAt).HasDefaultValueSql("NOW(6)");
        modelBuilder.Entity<CartItem>()
            .Property(c => c.CreatedAt).HasDefaultValueSql("NOW(6)");

        // ── CartItem relationship ──────────────────────────
        modelBuilder.Entity<CartItem>()
            .HasOne(c => c.User)
            .WithMany(u => u.CartItems)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Seed Categories ────────────────────────────────
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Nước khoáng",
                Description = "Nước khoáng thiên nhiên",
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Category
            {
                Id = 2,
                Name = "Nước tinh khiết",
                Description = "Nước tinh khiết đóng chai",
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Category
            {
                Id = 3,
                Name = "Nước có gas",
                Description = "Nước khoáng có gas",
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );

        // ── Seed Users ─────────────────────────────────────
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "Administrator",
                Email = "admin@gmail.com",
                Phone = "0900000000",
                Address = "TP. Hồ Chí Minh",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = Role.Admin,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new User
            {
                Id = 2,
                FullName = "Nguyễn Quốc Huy",
                Email = "user@gmail.com",
                Phone = "0901234567",
                Address = "123 Nguyễn Huệ, Q.1, TP.HCM",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                Role = Role.User,
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );

        // ── Seed Products ──────────────────────────────────
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "La Vie 500ml",
                Description = "Nước khoáng thiên nhiên 500ml",
                Price = 6000,
                StockQuantity = 500,
                LowStockThreshold = 50,
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 2,
                Name = "La Vie 1.5L",
                Description = "Nước khoáng thiên nhiên 1.5L",
                Price = 12000,
                StockQuantity = 300,
                LowStockThreshold = 30,
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 3,
                Name = "La Vie 19L",
                Description = "Bình nước khoáng 19L",
                Price = 65000,
                StockQuantity = 50,
                LowStockThreshold = 10,
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 4,
                Name = "Aquafina 500ml",
                Description = "Nước tinh khiết 500ml",
                Price = 6000,
                StockQuantity = 500,
                LowStockThreshold = 50,
                CategoryId = 2,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 5,
                Name = "Aquafina 1.5L",
                Description = "Nước tinh khiết 1.5L",
                Price = 11000,
                StockQuantity = 300,
                LowStockThreshold = 30,
                CategoryId = 2,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 6,
                Name = "Perrier 330ml",
                Description = "Nước khoáng có gas 330ml",
                Price = 25000,
                StockQuantity = 200,
                LowStockThreshold = 20,
                CategoryId = 3,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 7,
                Name = "Bình Aquafina 20L",
                Description = "Bình nước tinh khiết 20L",
                Price = 60000,
                StockQuantity = 80,
                LowStockThreshold = 10,
                CategoryId = 2,
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );
    }

    // ── Auto set CreatedAt ─────────────────────────────────
    public override int SaveChanges()
    {
        SetTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        SetTimestamps();
        return base.SaveChangesAsync(ct);
    }

    private void SetTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added))
        {
            try
            {
                entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            }
            catch { /* entity không có CreatedAt */ }
        }
    }
}