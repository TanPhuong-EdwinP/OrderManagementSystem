using Microsoft.EntityFrameworkCore;
using Order_Management_System.DTOs.Auth;
using Order_Management_System.Models;
using Order_Management_System.Services.Interfaces;
using Order_Management_System.Data;
using BCrypt.Net;

namespace Order_Management_System.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.User   
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = "registered"
        };
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
            throw new Exception("User not found");

        bool valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!valid)
            throw new Exception("Wrong password");

        return new AuthResponse
        {
            Token = "login-success"
        };
    }
}