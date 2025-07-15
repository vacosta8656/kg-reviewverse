using KgReviewverse.Common.Models.Entities;
using KgReviewverse.Backend.Services.Interfaces;
using KgReviewverse.Backend.Repositories.Interfaces;

namespace KgReviewverse.Backend.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> RegisterUserAsync(User user)
    {
        if (await _userRepository.UserExistsAsync(user.Email))
            throw new InvalidOperationException("User with this email already exists.");

        return await _userRepository.AddUserAsync(user);
    }

    public Task<User?> GetUserByEmailAsync(string email) => _userRepository.GetByEmailAsync(email);

    public Task<bool> UserExistsAsync(string email) => _userRepository.UserExistsAsync(email);
}
