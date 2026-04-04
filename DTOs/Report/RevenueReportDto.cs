namespace Order_Management_System.DTOs.Report;

public class RevenueReportDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int ShippedOrders { get; set; }
}