using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TextAdventureApi.Dtos;
using TextAdventureApi.Models;
using TextAdventureApi.Security;

namespace TextAdventureApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly List<User> _users = new();

        // simple secret voor schoolproject
        private const string JwtSecretKey = "THIS_IS_A_DEMO_SECRET_KEY_CHANGE_ME";
        private const string JwtIssuer = "TextAdventureApi";
        private const string JwtAudience = "TextAdventureGame";

        public bool UsernameExists(string username)
            => _users.Any(u => u.Username == username);

        public User Register(RegisterRequest request)
        {
            if (UsernameExists(request.Username))
                throw new InvalidOperationException("Username already exists");

            var user = new User
            {
                Username = request.Username,
                PasswordHash = Sha256Hasher.Hash(request.Password),
                Role = request.Role
            };

            _users.Add(user);
            return user;
        }

        public LoginResponse? Login(LoginRequest request, out string? error, out bool lockedOut)
        {
            error = null;
            lockedOut = false;

            var user = _users.SingleOrDefault(u => u.Username == request.Username);

            if (user == null)
            {
                error = "Invalid username or password.";
                return null;
            }

            if (user.IsLockedOut)
            {
                lockedOut = true;
                error = "Account is locked.";
                return null;
            }

            var hash = Sha256Hasher.Hash(request.Password);
            if (hash != user.PasswordHash)
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 3)
                {
                    user.IsLockedOut = true;
                    lockedOut = true;
                    error = "Account locked after too many failed attempts.";
                }
                else
                {
                    error = "Invalid username or password.";
                }

                return null;
            }

            // Succes: teller resetten
            user.FailedLoginAttempts = 0;
            user.IsLockedOut = false;

            var token = GenerateJwt(user);

            return new LoginResponse
            {
                Token = token,
                Role = user.Role
            };
        }

        private string GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
