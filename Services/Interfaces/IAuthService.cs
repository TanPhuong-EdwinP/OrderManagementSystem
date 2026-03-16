using Order_Management_System.DTOs.Auth;

namespace Order_Management_System.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> Login(LoginRequest request);
    Task<AuthResponse> Register(RegisterRequest request);
}