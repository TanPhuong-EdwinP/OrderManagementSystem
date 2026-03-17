using Microsoft.EntityFrameworkCore;
using Order_Management_System.Data;
using Order_Management_System.DTOs.Order;
using Order_Management_System.Models;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    public OrderService(AppDbContext db) => _db = db;

    public async Task<List<OrderDto>> GetAll()
    {
        return await _db.Orders
            .Include(o => o.OrderItems).ThenInclude(i => i.Product)
            .Select(o => MapDto(o)).ToListAsync();
    }

    public async Task<List<OrderDto>> GetByUserId(int userId)
    {
        return await _db.Orders
            .Include(o => o.OrderItems).ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => MapDto(o)).ToListAsync();
    }

    public async Task<OrderDto?> GetById(int id)
    {
        var o = await _db.Orders
            .Include(o => o.OrderItems).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
        return o == null ? null : MapDto(o);
    }

    public async Task<OrderDto> Create(CreateOrderDto dto)
    {
        var order = new Order
        {
            UserId = dto.UserId,
            ShippingName = dto.ShippingName,
            ShippingPhone = dto.ShippingPhone,
            ShippingAddress = dto.ShippingAddress,
            Note = dto.Note,
            Status = OrderStatus.Pending,
            TotalAmount = dto.Total
        };

        if (dto.Items != null && dto.Items.Any())
        {
            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _db.Products.FindAsync(item.ProductId)
                    ?? throw new Exception($"Sản phẩm ID {item.ProductId} không tồn tại.");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Sản phẩm '{product.Name}' không đủ hàng.");

                product.StockQuantity -= item.Quantity;
                total += product.Price * item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                });
            }

            order.TotalAmount = total;
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return (await GetById(order.Id))!;
    }

    public async Task<OrderDto?> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return null;
        if (Enum.TryParse<OrderStatus>(dto.Status, true, out var s))
            order.Status = s;
        await _db.SaveChangesAsync();
        return await GetById(id);
    }

    // ✅ Hủy đơn — chỉ được khi Pending hoặc Confirmed
    public async Task<bool> CancelOrder(int orderId, int userId)
    {
        var order = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null || order.UserId != userId) return false;
        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
            throw new Exception("Chỉ có thể hủy đơn khi đang ở trạng thái Chờ xác nhận hoặc Đã xác nhận.");

        // ✅ Hoàn lại tồn kho cho từng sản phẩm
        foreach (var item in order.OrderItems)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            if (product != null)
                product.StockQuantity += item.Quantity;
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private static OrderDto MapDto(Order o) => new()
    {
        Id = o.Id,
        UserId = o.UserId,
        Total = o.TotalAmount,
        Status = o.Status.ToString(),
        ShippingName = o.ShippingName,
        ShippingPhone = o.ShippingPhone,
        ShippingAddress = o.ShippingAddress,
        Note = o.Note,
        CreatedAt = o.CreatedAt,
        Items = o.OrderItems.Select(i => new OrderItemDetailDto
        {
            ProductId = i.ProductId,
            ProductName = i.Product?.Name ?? "",
            Quantity = i.Quantity,
            Price = i.Price,
            Subtotal = i.Price * i.Quantity
        }).ToList()
    };
}