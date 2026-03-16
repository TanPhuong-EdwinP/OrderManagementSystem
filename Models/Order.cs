using Order_Management_System.Models;

namespace Order_Management_System.Models;

public enum OrderStatus
{
    Pending, Confirmed, Shipped, Delivered, Cancelled
}

public class Order
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public string? ShippingAddress { get; set; }

    // ✅ Không gán = DateTime.UtcNow ở đây
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}