namespace Order_Management_System.DTOs.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }
}
