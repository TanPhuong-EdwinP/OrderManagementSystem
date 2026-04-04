using Microsoft.EntityFrameworkCore;
using Order_Management_System.Data;
using Order_Management_System.DTOs.Report;
using Order_Management_System.Models;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    // Chỉ tính đơn Shipped + Delivered
    private static readonly OrderStatus[] ValidStatuses =
        { OrderStatus.Shipped, OrderStatus.Delivered };

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RevenueReportDto> GetRevenue()
    {
        var rows = await _context.Orders
            .Where(o => ValidStatuses.Contains(o.Status))
            .Select(o => new { o.TotalAmount, o.Status })
            .ToListAsync();

        return new RevenueReportDto
        {
            TotalRevenue    = rows.Sum(o => o.TotalAmount),
            TotalOrders     = rows.Count,
            DeliveredOrders = rows.Count(o => o.Status == OrderStatus.Delivered),
            ShippedOrders   = rows.Count(o => o.Status == OrderStatus.Shipped)
        };
    }

    public async Task<List<TopProductDto>> GetTopProducts()
    {
        var result = await (
            from item in _context.OrderItems
            join ord  in _context.Orders  on item.OrderId  equals ord.Id
            join prod in _context.Products on item.ProductId equals prod.Id
            where ValidStatuses.Contains(ord.Status)
            group new { item, prod } by new { item.ProductId, prod.Name } into g
            select new TopProductDto
            {
                ProductId   = g.Key.ProductId,
                ProductName = g.Key.Name,
                Quantity    = g.Sum(x => x.item.Quantity),
                Revenue     = g.Sum(x => x.item.Quantity * x.item.Price)
            }
        )
        .OrderByDescending(x => x.Quantity)
        .Take(5)
        .ToListAsync();

        return result;
    }
}