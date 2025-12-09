using TextAdventureApi.Dtos;
using TextAdventureApi.Services;

namespace TextAdventureApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Services registreren
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Onze eigen service
            builder.Services.AddSingleton<IAuthService, AuthService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //Minimal API endpoints hier

            // POST /api/auth/register
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

            app.Run();
        }
    }
}
