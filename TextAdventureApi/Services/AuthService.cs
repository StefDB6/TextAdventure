using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TextAdventureApi.Data;
using TextAdventureApi.Dtos;
using TextAdventureApi.Models;
using TextAdventureApi.Options;
using TextAdventureApi.Security;

namespace TextAdventureApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly TextAdventureDbContext _db;
        private readonly JwtOptions _jwtOptions;

        public AuthService(TextAdventureDbContext db, IOptions<JwtOptions> jwtOptions)
        {
            _db = db;
            _jwtOptions = jwtOptions.Value;
        }

        public bool UsernameExists(string username)
        {
            return _db.Users.Any(u => u.Username == username);
        }

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

            _db.Users.Add(user);
            _db.SaveChanges();

            return user;
        }

        public LoginResponse? Login(LoginRequest request, out string? error, out bool lockedOut)
        {
            error = null;
            lockedOut = false;

            var user = _db.Users.SingleOrDefault(u => u.Username == request.Username);

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

                _db.SaveChanges();
                return null;
            }

            // Succes: teller resetten
            user.FailedLoginAttempts = 0;
            user.IsLockedOut = false;
            _db.SaveChanges();

            var token = GenerateJwt(user);

            return new LoginResponse
            {
                Token = token,
                Role = user.Role
            };
        }

        private string GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("uid", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
