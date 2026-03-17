using Microsoft.EntityFrameworkCore;
using Order_Management_System.Services.Interfaces;
using Order_Management_System.Data;
using Order_Management_System.DTOs.Report;
using Order_Management_System.Models;


namespace Order_Management_System.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RevenueReportDto> GetRevenue()
    {
        var validStatuses = new[] { OrderStatus.Shipped, OrderStatus.Delivered };

        var total = await _context.Orders
            .Where(o => validStatuses.Contains(o.Status))
            .SumAsync(o => o.TotalAmount);

        return new RevenueReportDto
        {
            TotalRevenue = total
        };
    }

    public async Task<List<TopProductDto>> GetTopProducts()
    {
        var validStatuses = new[] { OrderStatus.Shipped, OrderStatus.Delivered };

        var result = await _context.OrderItems
            .Include(x => x.Order)
            .Where(x => validStatuses.Contains(x.Order.Status))
            .GroupBy(x => x.ProductId)
            .Select(g => new TopProductDto
            {
                ProductId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.Quantity)
            .Take(5)
            .ToListAsync();

        return result;
    }
}