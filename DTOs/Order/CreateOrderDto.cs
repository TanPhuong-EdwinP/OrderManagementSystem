namespace Order_Management_System.DTOs.Order;

public class CreateOrderDto
{
    public int UserId { get; set; }

    public decimal Total { get; set; }
}