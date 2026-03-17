using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Order_Management_System.Data;
using Order_Management_System.DTOs.Auth;
using Order_Management_System.Models;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new Exception("Email này đã được sử dụng.");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.User
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return BuildResponse(user);
    }

    private AuthResponse BuildResponse(User user) => new()
    {
        Id = user.Id,
        Token = GenerateToken(user),
        Email = user.Email,
        FullName = user.FullName,
        Phone = user.Phone,
        Address = user.Address,
        Role = user.Role.ToString()
    };

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            throw new Exception("Email không tồn tại.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Mật khẩu không đúng.");

        return new AuthResponse
        {
            Token = GenerateToken(user),
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email,          user.Email),
            new Claim(ClaimTypes.Role,           user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                                    double.Parse(_config["Jwt:ExpireMinutes"]!.ToString())),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}