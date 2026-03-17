using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order_Management_System.DTOs.User;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _user;
    public UserController(IUserService user) => _user = user;

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
}