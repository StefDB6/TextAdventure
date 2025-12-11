using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TestRaiders_TextAdventure.Core.Interfaces;
using TestRaiders_TextAdventure.Core.Models;

namespace TestRaiders_TextAdventure
{
    internal class Program
    {
        // PAS DIT AAN als jouw API op een andere poort draait
        private const string ApiBaseUrl = "https://localhost:7114";

        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            GameSetup.RegisterDependencies(services);

            // Zorg dat de encrypted rooms bestaan (.enc files)
            TestRaiders_TextAdventure.Core.Encryption.EncryptedRoomGenerator.EnsureEncryptedRoomsExist();

            // 1) World initialiseren
            var roomsManager = GameSetup.InitializeWorld();

            // 2) Eerst registreren bij de API
            await RegisterWithApiAsync();

            // 3) Dan inloggen (blijft vragen tot succes of lockout)
            var token = await LoginWithApiAsync();
            if (token == null)
            {
                Console.WriteLine("Could not log in. Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Login OK, token (ingekort): {token[..20]}...");
            Console.WriteLine();

            // 4) Fetch keyshare from API
            var keyshare = await GetKeyshareFromApiAsync(token);
            if (keyshare == null)
            {
                Console.WriteLine("Could not retrieve keyshare.");
                return;
            }

            // 5) Store in RoomsManager
            roomsManager.Keyshare = keyshare;
            roomsManager.JwtToken = token;

            // 6) Game starten
            var game = new Game(roomsManager);
            game.Start();
          
            Console.ReadKey();
        }

        private static async Task<string?> GetKeyshareFromApiAsync(string token)
        {
            using var http = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await http.GetAsync("/api/keys/keyshare/main");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to retrieve keyshare: {(int)response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }

            var json = await response.Content.ReadFromJsonAsync<KeyshareDto>();
            return json?.KeyShare;
        }

        private class KeyshareDto
        {
            public string RoomId { get; set; } = "";
            public string KeyShare { get; set; } = "";
        }

        private static async Task RegisterWithApiAsync()
        {
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
                role = 0 // 0 = Player (Role.Player in jouw API)
            };

            var response = await http.PostAsJsonAsync("/api/auth/register", body);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Registration successful!");
            }
            else if ((int)response.StatusCode == 409)
            {
                Console.WriteLine("Username already exists, we gaan gewoon verder met login.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Registration failed: {(int)response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine(error);
            }

            Console.WriteLine();
        }

        private static async Task<string?> LoginWithApiAsync()
        {
            using var http = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };

            while (true)
            {
                Console.WriteLine("=== Login ===");
                Console.Write("Username: ");
                var username = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();

                var body = new
                {
                    username,
                    password
                };

                var response = await http.PostAsJsonAsync("/api/auth/login", body);

                if (response.IsSuccessStatusCode)
                {
                    // 200 OK → token ophalen
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.Token))
                    {
                        Console.WriteLine("Login failed: no token received from API.");
                        return null;
                    }

                    Console.WriteLine("Login successful!");
                    return loginResponse.Token;
                }

                // 423 = locked door API na 3 mislukte attempts
                if ((int)response.StatusCode == 423)
                {
                    Console.WriteLine("Your account has been locked after too many failed attempts.");
                    return null;
                }

                // 401 = verkeerde username/password
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Wrong username or password. Please try again.");
                    Console.WriteLine();
                    continue; // opnieuw vragen
                }

                // Andere fouten (400, 500, ...)
                var txt = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login failed: {(int)response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine(txt);
                return null;
            }
        }

        // DTO voor het antwoord van /api/auth/login
        private class LoginResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public int Role { get; set; }
        }
    }
}
