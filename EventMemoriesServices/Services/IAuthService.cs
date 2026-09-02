using DalEntities;
using EventMemoriesServices.DTOs;

namespace EventMemoriesServices.Services
{
    public interface IAuthService
    {
        Task<ApplicationUser> GetUserAsync(string email);
        Task<LoginResult> Login(LoginRequest model);
        Task<LoginResult> Register(RegisterRequest registerRequest);
        Task<LoginResult> RefreshToken(Guid userId);
        Task Logout();
        Task<ApplicationUser> GetCurrentUser(Guid userId);  
        Task<bool> ChangePassword(Guid userId, ChangePasswordRequest model);
    }
}