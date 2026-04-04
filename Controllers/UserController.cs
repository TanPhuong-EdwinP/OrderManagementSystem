using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order_Management_System.Data;
using Order_Management_System.DTOs.User;
using Order_Management_System.Models;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _user;
    private readonly AppDbContext _db;

    public UserController(IUserService user, AppDbContext db)
    {
        _user = user;
        _db = db;
    }

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await _user.GetProfile(UserId);
        return profile == null ? NotFound() : Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var result = await _user.UpdateProfile(UserId, dto);
        return result == null ? NotFound() : Ok(result);
    }

    // ✅ Admin lấy danh sách khách hàng kèm số đơn
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _db.Users
            .Where(u => u.Role == Role.User)
            .Select(u => new UserProfileDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Address = u.Address,
                OrderCount = u.Orders.Count
            }).ToListAsync();
        return Ok(users);
    }

    // ✅ Admin lấy danh sách Admin
    [HttpGet("admins")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAdmins()
    {
        var admins = await _db.Users
            .Where(u => u.Role == Role.Admin)
            .Select(u => new UserProfileDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Address = u.Address,
                OrderCount = u.Orders.Count
            }).ToListAsync();
        return Ok(admins);
    }
}