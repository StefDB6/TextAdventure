using TextAdventureApi.Models;

namespace TextAdventureApi.Dtos
{
    public class LoginResponse
    {
        public string Token { get; set; } = default!;
        public Role Role { get; set; }
    }
}
