using KgReviewverse.Common.Models.Entities;

namespace KgReviewverse.Backend.Services.Interfaces;

public interface IUserService
{
    Task<User> RegisterUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);
}