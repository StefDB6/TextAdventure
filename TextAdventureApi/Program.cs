using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TextAdventureApi.Data;
using TextAdventureApi.Dtos;
using TextAdventureApi.Options;
using TextAdventureApi.Services;

namespace TextAdventureApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext 
            builder.Services.AddDbContext<TextAdventureDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //JwtOptions uit appsettings 
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection("Jwt"));

            var jwtOptions = builder.Configuration
                .GetSection("Jwt")
                .Get<JwtOptions>() ?? new JwtOptions();

            // JWT Authenticatie
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.Key))
                    };
                });

            builder.Services.AddAuthorization();

            //Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Services 
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            //Endpoints 

            // 4.1 Registreren (POST /api/auth/register)
            app.MapPost("/api/auth/register",
                (RegisterRequest request, IAuthService authService) =>
                {
                    if (string.IsNullOrWhiteSpace(request.Username) ||
                        string.IsNullOrWhiteSpace(request.Password))
                    {
                        return Results.BadRequest("Username and password are required.");
                    }

                    if (authService.UsernameExists(request.Username))
                    {
                        return Results.Conflict("Username already exists");
                    }

                    var user = authService.Register(request);

                    return Results.Created($"/api/users/{user.Id}", new
                    {
                        user.Id,
                        user.Username,
                        Role = user.Role.ToString()
                    });
                });

            // 4.2 Inloggen (POST /api/auth/login)
            app.MapPost("/api/auth/login",
                (LoginRequest request, IAuthService authService) =>
                {
                    if (string.IsNullOrWhiteSpace(request.Username) ||
                        string.IsNullOrWhiteSpace(request.Password))
                    {
                        return Results.BadRequest("Username and password are required.");
                    }

                    var result = authService.Login(request, out var error, out var lockedOut);

                    if (lockedOut)
                    {
                        // 423 Locked na 3 foute pogingen
                        return Results.StatusCode(423);
                    }

                    if (result == null)
                    {
                        // fout username/password
                        return Results.Unauthorized();
                    }

                    return Results.Ok(result); // { token, role }
                });

            // 4.3 Huidige user (GET /api/auth/me)
            app.MapGet("/api/auth/me", (HttpContext http) =>
            {
                var principal = http.User;

                if (principal?.Identity?.IsAuthenticated != true)
                    return Results.Unauthorized();

                var username = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var userId = principal.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                return Results.Ok(new
                {
                    Id = userId,
                    Username = username,
                    Role = role
                });
            })
            .RequireAuthorization();

            app.MapGet("/api/keys/keyshare/{roomId}", async (string roomId, HttpContext http, TextAdventureDbContext db) =>
            {
                var user = http.User;

                if (user?.Identity?.IsAuthenticated != true)
                    return Results.Unauthorized();

                // Claims
                var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                // Keyshare zoeken
                var keyshare = await db.KeyShares
                    .FirstOrDefaultAsync(k => k.RoomId.ToLower() == roomId.ToLower());

                if (keyshare == null)
                    return Results.NotFound("Room has no keyshare.");

                // Rolcontrole
                bool allowed =
                    role == "Admin" ||
                    (role == "Player" && keyshare.MinRole == "Player");

                if (!allowed)
                    return Results.Forbid();

                // OK
                return Results.Ok(new
                {
                    RoomId = keyshare.RoomId,
                    KeyShare = keyshare.Share
                });
            })
            .RequireAuthorization();

            app.Run();
        }
    }
}
