using TextAdventureApi.Models;

namespace TextAdventureApi.Dtos
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public Role Role { get; set; }
    }
}
