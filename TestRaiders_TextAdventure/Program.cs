using System.Net.Http.Json;
using TestRaiders_TextAdventure.Core.Interfaces;
using TestRaiders_TextAdventure.Core.Models;

namespace TestRaiders_TextAdventure
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            GameSetup.RegisterDependencies(services);

            // 1) World initialiseren
            var roomsManager = GameSetup.InitializeWorld();

            // 2) EERST: login / register via API
            await RegisterWithApiAsync();

            // 3) Dan pas de game maken
            var game = new Game(roomsManager);

            // 4) Game loop starten
            Console.WriteLine("Welcome to TestRaiders! Type 'help' for commands.");
            game.Start();

            Console.WriteLine("Game exited. Press any key to close...");
        }

        private static async Task RegisterWithApiAsync()
        {
            const string ApiBaseUrl = "https://localhost:7114";

            using var http = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };

            Console.WriteLine("=== Register new account ===");
            Console.Write("Choose a username: ");
            var username = Console.ReadLine();

            Console.Write("Choose a password: ");
            var password = Console.ReadLine();

            var body = new
            {
                username,
                password,
                role = 0 // 0 = Player
            };

            var response = await http.PostAsJsonAsync("/api/auth/register", body);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Registration successful!");
            }
            else
            {
                var errorText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Registration failed: {(int)response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine(errorText);
            }

            Console.WriteLine();
        }
    }
}
