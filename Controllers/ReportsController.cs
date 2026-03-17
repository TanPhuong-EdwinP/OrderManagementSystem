using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]   
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue()
    {
        var result = await _reportService.GetRevenue();
        return Ok(result);
    }

    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopProducts()
    {
        var result = await _reportService.GetTopProducts();
        return Ok(result);
    }
}