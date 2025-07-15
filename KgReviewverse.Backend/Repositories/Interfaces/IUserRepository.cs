using KgReviewverse.Common.Models.Entities;

namespace KgReviewverse.Backend.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddUserAsync(User user);
    Task<bool> UserExistsAsync(string email);
}