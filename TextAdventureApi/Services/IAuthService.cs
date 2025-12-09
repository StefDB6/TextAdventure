using TextAdventureApi.Models;
using TextAdventureApi.Dtos;

namespace TextAdventureApi.Services
{
    public interface IAuthService
    {
        User Register(RegisterRequest request);
        bool UsernameExists(string username);

        LoginResponse? Login(LoginRequest request, out string? error, out bool lockedOut);
    }
}
