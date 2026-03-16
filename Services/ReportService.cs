using Microsoft.EntityFrameworkCore;
using Order_Management_System.Services.Interfaces;
using Order_Management_System.Data;
using Order_Management_System.DTOs.Report;


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
        var total = await _context.Orders.SumAsync(o => o.TotalAmount);

        return new RevenueReportDto
        {
            TotalRevenue = total
        };
    }

    public async Task<List<TopProductDto>> GetTopProducts()
    {
        var result = await _context.OrderItems
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