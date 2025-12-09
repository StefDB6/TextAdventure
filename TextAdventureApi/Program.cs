using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            app.Run();
        }
    }
}
