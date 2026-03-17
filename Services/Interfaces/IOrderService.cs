using Order_Management_System.DTOs.Order;

namespace Order_Management_System.Services.Interfaces;

public interface IOrderService
{
    Task<List<OrderDto>> GetAll();
    Task<List<OrderDto>> GetByUserId(int userId);
    Task<OrderDto?> GetById(int id);
    Task<OrderDto> Create(CreateOrderDto dto);
    Task<OrderDto?> UpdateStatus(int id, UpdateOrderStatusDto dto);
    Task<bool> CancelOrder(int orderId, int userId);
}