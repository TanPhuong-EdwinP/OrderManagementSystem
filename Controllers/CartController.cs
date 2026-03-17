using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order_Management_System.DTOs.Cart;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cart;
    public CartController(ICartService cart) => _cart = cart;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _cart.GetCart(UserId));

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddToCartDto dto)
    {
        try { return Ok(await _cart.AddToCart(UserId, dto)); }
        catch (Exception e) { return BadRequest(new { message = e.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCartDto dto)
    {
        var result = await _cart.UpdateCart(UserId, id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        var result = await _cart.RemoveFromCart(UserId, id);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        await _cart.ClearCart(UserId);
        return Ok();
    }
}