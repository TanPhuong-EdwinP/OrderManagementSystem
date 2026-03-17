using Microsoft.EntityFrameworkCore;
using Order_Management_System.Data;
using Order_Management_System.DTOs.User;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) => _db = db;

    public async Task<UserProfileDto?> GetProfile(int userId)
    {
        var u = await _db.Users.FindAsync(userId);
        if (u == null) return null;
        return new UserProfileDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            Address = u.Address
        };
    }

    public async Task<UserProfileDto?> UpdateProfile(int userId, UpdateProfileDto dto)
    {
        var u = await _db.Users.FindAsync(userId);
        if (u == null) return null;
        u.FullName = dto.FullName;
        u.Phone = dto.Phone;
        u.Address = dto.Address;
        await _db.SaveChangesAsync();
        return new UserProfileDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            Address = u.Address
        };
    }
}