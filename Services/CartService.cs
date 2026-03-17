using Microsoft.EntityFrameworkCore;
using Order_Management_System.Data;
using Order_Management_System.DTOs.Cart;
using Order_Management_System.Models;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _db;
    public CartService(AppDbContext db) => _db = db;

    public async Task<List<CartItemDto>> GetCart(int userId)
    {
        return await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .Select(c => new CartItemDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                Price = c.Product.Price,
                Quantity = c.Quantity,
                Subtotal = c.Product.Price * c.Quantity
            }).ToListAsync();
    }

    public async Task<CartItemDto> AddToCart(int userId, AddToCartDto dto)
    {
        // Kiểm tra sản phẩm tồn tại
        var product = await _db.Products.FindAsync(dto.ProductId)
            ?? throw new Exception("Sản phẩm không tồn tại.");

        // Nếu đã có trong giỏ → tăng số lượng
        var existing = await _db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        if (existing != null)
        {
            existing.Quantity += dto.Quantity;
            await _db.SaveChangesAsync();
            return new CartItemDto
            {
                Id = existing.Id,
                ProductId = existing.ProductId,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = existing.Quantity,
                Subtotal = product.Price * existing.Quantity
            };
        }

        var item = new CartItem
        {
            UserId = userId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        };
        _db.CartItems.Add(item);
        await _db.SaveChangesAsync();

        return new CartItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = product.Name,
            Price = product.Price,
            Quantity = item.Quantity,
            Subtotal = product.Price * item.Quantity
        };
    }

    public async Task<CartItemDto?> UpdateCart(int userId, int cartItemId, UpdateCartDto dto)
    {
        var item = await _db.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
        if (item == null) return null;

        item.Quantity = dto.Quantity;
        await _db.SaveChangesAsync();

        return new CartItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Price = item.Product.Price,
            Quantity = item.Quantity,
            Subtotal = item.Product.Price * item.Quantity
        };
    }

    public async Task<bool> RemoveFromCart(int userId, int cartItemId)
    {
        var item = await _db.CartItems
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
        if (item == null) return false;
        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task ClearCart(int userId)
    {
        var items = await _db.CartItems.Where(c => c.UserId == userId).ToListAsync();
        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync();
    }
}