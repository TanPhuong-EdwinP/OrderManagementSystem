namespace Order_Management_System.DTOs.Auth;

public class RegisterRequest
{
    public string FullName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}