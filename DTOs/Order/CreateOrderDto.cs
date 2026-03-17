namespace Order_Management_System.DTOs.Order;

public class CreateOrderDto
{
    public int UserId { get; set; }
    public string ShippingName { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Note { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDto2>? Items { get; set; }
}

public class OrderItemDto2
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

