using Microsoft.EntityFrameworkCore;
using Order_Management_System.Services.Interfaces;
using Order_Management_System.Data;
using Order_Management_System.DTOs.Order;
using Order_Management_System.Models;

namespace Order_Management_System.Services; 

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> GetAll()
    {
        return await _context.Orders
            .Select(o => new OrderDto
            {
                Id = o.Id,
                // FIX: Sửa .Total thành .TotalAmount theo Model của bạn
                Total = o.TotalAmount,
                Status = o.Status.ToString()
            }).ToListAsync();
    }

    public async Task<OrderDto?> GetById(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null) return null;

        return new OrderDto
        {
            Id = order.Id,
            // FIX: Sửa .Total thành .TotalAmount
            Total = order.TotalAmount,
            Status = order.Status.ToString()
        };
    }

    public async Task<OrderDto> Create(CreateOrderDto dto)
    {
        var order = new Order
        {
            UserId = dto.UserId,
            // FIX: Sửa .Total thành .TotalAmount
            TotalAmount = dto.Total,
            // FIX: Gán Enum trực tiếp thay vì dùng chuỗi "pending"
            Status = OrderStatus.Pending
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            Total = order.TotalAmount,
            Status = order.Status.ToString()
        };
    }

    public async Task<OrderDto?> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null) return null;

        // FIX: Chuyển đổi từ string (dto.Status) sang Enum (order.Status)
        if (Enum.TryParse<OrderStatus>(dto.Status, true, out var newStatus))
        {
            order.Status = newStatus;
        }

        await _context.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            Total = order.TotalAmount,
            Status = order.Status.ToString()
        };
    }
}