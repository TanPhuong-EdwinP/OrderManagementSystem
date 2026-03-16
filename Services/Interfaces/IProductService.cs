using Order_Management_System.DTOs.Product;


namespace Order_Management_System.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetAll();
    Task<ProductDto?> GetById(int id);
    Task<ProductDto> Create(CreateProductDto dto);
    Task<ProductDto?> Update(int id, UpdateProductDto dto);
    Task<bool> Delete(int id);
}