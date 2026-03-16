namespace Order_Management_System.DTOs.Product;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }
}