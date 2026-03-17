using Microsoft.EntityFrameworkCore;
using Order_Management_System.DTOs.Product;
using Order_Management_System.Models;
using Order_Management_System.Services.Interfaces;
using Order_Management_System.Data;


namespace Order_Management_System.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetAll()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            }).ToListAsync();
    }

    public async Task<ProductDto?> GetById(int id)
    {
        var p = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (p == null) return null;

        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name
        };
    }

    public async Task<ProductDto> Create(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            StockQuantity = dto.Stock,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId
        };
    }

    public async Task<ProductDto?> Update(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return null;

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.StockQuantity = dto.Stock;

        await _context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId
        };
    }

    public async Task<bool> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }
}