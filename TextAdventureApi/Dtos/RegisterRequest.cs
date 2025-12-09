using TextAdventureApi.Models;

namespace TextAdventureApi.Dtos
{
    public class RegisterRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public Role Role { get; set; } = Role.Player; // default
    }
}
