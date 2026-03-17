using Order_Management_System.DTOs.Cart;

namespace Order_Management_System.Services.Interfaces;

public interface ICartService
{
    Task<List<CartItemDto>> GetCart(int userId);
    Task<CartItemDto> AddToCart(int userId, AddToCartDto dto);
    Task<CartItemDto?> UpdateCart(int userId, int cartItemId, UpdateCartDto dto);
    Task<bool> RemoveFromCart(int userId, int cartItemId);
    Task ClearCart(int userId);
}