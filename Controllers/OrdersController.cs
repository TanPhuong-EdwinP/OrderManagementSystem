using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order_Management_System.DTOs.Order;
using Order_Management_System.Models;
using Order_Management_System.Services.Interfaces;


using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order_Management_System.DTOs.Order;
using Order_Management_System.Services.Interfaces;

namespace Order_Management_System.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService) => _orderService = orderService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string UserRole => User.FindFirstValue(ClaimTypes.Role) ?? "";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (UserRole == "Admin") return Ok(await _orderService.GetAll());
        return Ok(await _orderService.GetByUserId(UserId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetById(id);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            var order = await _orderService.Create(dto);
            return Ok(order);
        }
        catch (Exception e) { return BadRequest(new { message = e.Message }); }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        var order = await _orderService.UpdateStatus(id, dto);
        return order == null ? NotFound() : Ok(order);
    }

    // ✅ User tự hủy đơn
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var result = await _orderService.CancelOrder(id, UserId);
            return result ? Ok(new { message = "Hủy đơn thành công." }) : NotFound();
        }
        catch (Exception e) { return BadRequest(new { message = e.Message }); }
    }
}
/*
namespace Order_Management_System.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (role == "Admin")
        {
            var all = await _orderService.GetAll();
            return Ok(all);
        }

        // Lấy userId từ JWT claims
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized(new { message = "Không xác định được user." });

        var myOrders = await _orderService.GetByUserId(userId);
        return Ok(myOrders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetById(id);
        if (order == null)
            return NotFound(new { message = $"Không tìm thấy đơn hàng ID {id}." });
        return Ok(order);
    }

    // ✅ Ai đăng nhập cũng tạo được đơn
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        var order = await _orderService.Create(dto);
        return Ok(order);
    }

    // ✅ Chỉ Admin mới đổi trạng thái
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var order = await _orderService.UpdateStatus(id, dto);
        if (order == null)
            return NotFound(new { message = $"Không tìm thấy đơn hàng ID {id}." });
        return Ok(order);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var result = await _orderService.CancelOrder(id, UserId);
            return result ? Ok(new { message = "Hủy đơn thành công." }) : NotFound();
        }
        catch (Exception e) { return BadRequest(new { message = e.Message }); }
    }
}
*/