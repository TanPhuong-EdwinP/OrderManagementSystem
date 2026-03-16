using Order_Management_System.DTOs.Report;

namespace Order_Management_System.Services.Interfaces;

public interface IReportService
{
    Task<RevenueReportDto> GetRevenue();
    Task<List<TopProductDto>> GetTopProducts();
}