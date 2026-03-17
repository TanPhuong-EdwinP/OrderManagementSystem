namespace Order_Management_System.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ShippingName { get; set; }
    public string? ShippingPhone { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDetailDto> Items { get; set; } = new();
}

public class OrderItemDetailDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}