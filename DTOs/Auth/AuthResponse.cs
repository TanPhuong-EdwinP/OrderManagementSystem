namespace Order_Management_System.DTOs.Auth;

public class AuthResponse
{
    public int Id { get; set; }
    public string Token { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string FullName { get; set; } = default!; 
    public string   Phone { get; set; } = default!;
    public string Address { get; set; } = default!;
}