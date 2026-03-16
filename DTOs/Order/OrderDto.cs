namespace Order_Management_System.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}