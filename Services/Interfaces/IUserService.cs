using Order_Management_System.DTOs.User;

namespace Order_Management_System.Services.Interfaces;

public interface IUserService
{
    Task<UserProfileDto?> GetProfile(int userId);
    Task<UserProfileDto?> UpdateProfile(int userId, UpdateProfileDto dto);
}