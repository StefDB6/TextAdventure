using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TestRaiders_TextAdventure.Core.Models;

namespace TestRaiders_TextAdventure
{
    internal class Program
    {
        private const string ApiBaseUrl = "https://localhost:7114";

        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            GameSetup.RegisterDependencies(services);

            // Zorg dat de encrypted rooms bestaan (.enc files)
            TestRaiders_TextAdventure.Core.Encryption.EncryptedRoomGenerator.EnsureEncryptedRoomsExist();

            // 1) World initialiseren
            var roomsManager = GameSetup.InitializeWorld();

            // 2) Kies user of registreer
            var username = await ChooseOrRegisterUsernameAsync();
            if (username == null)
            {
                Console.WriteLine("Goodbye!");
                return;
            }

            // 3) Login loop (enkel password vragen)
            var token = await LoginWithApiAsync(username);
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
                Console.ReadKey();
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

        // USER lijst en optie voor registratie
        private static async Task<string?> ChooseOrRegisterUsernameAsync()
        {
            while (true)
            {
                var users = await GetUsersAsync();

                Console.WriteLine("=== Accounts ===");

                if (users.Count == 0)
                {
                    Console.WriteLine("No accounts found. You must register.\n");
                    var created = await RegisterWithApiAsync();
                    if (created != null) return created;

                    continue;
                }

                for (int i = 0; i < users.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {users[i]}");
                }

                Console.WriteLine("R) Register new account");
                Console.WriteLine("Q) Quit");
                Console.Write("Choose: ");

                var input = (Console.ReadLine() ?? "").Trim();

                if (input.Equals("Q", StringComparison.OrdinalIgnoreCase))
                    return null;

                if (input.Equals("R", StringComparison.OrdinalIgnoreCase))
                {
                    var created = await RegisterWithApiAsync();
                    if (created != null) return created;

                    continue;
                }

                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= users.Count)
                {
                    return users[choice - 1];
                }

                Console.WriteLine("Invalid choice. Try again.\n");
            }
        }

        private static async Task<List<string>> GetUsersAsync()
        {
            using var http = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };

            try
            {
                var res = await http.GetAsync("/api/auth/users");
                if (!res.IsSuccessStatusCode)
                    return new List<string>();

                var list = await res.Content.ReadFromJsonAsync<List<string>>();
                return list ?? new List<string>();
            }
            catch
            {
                Console.WriteLine("API not reachable. Is TextAdventureApi running?\n");
                return new List<string>();
            }
        }

        // REGISTER
        
        private static async Task<string?> RegisterWithApiAsync()
        {
            using var http = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };

            Console.WriteLine("\n=== Register new account ===");
            Console.Write("Choose a username: ");
            var username = (Console.ReadLine() ?? "").Trim();

            Console.Write("Choose a password: ");
            var password = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Username and password are required.\n");
                return null;
            }

            var body = new
            {
                username,
                password,
                role = 0
            };

            var response = await http.PostAsJsonAsync("/api/auth/register", body);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Registration successful!\n");
                return username;
            }

            if ((int)response.StatusCode == 409)
            {
                Console.WriteLine("Username already exists.\n");
                return null;
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Registration failed: {(int)response.StatusCode} - {response.ReasonPhrase}");
            Console.WriteLine(error);
            Console.WriteLine();
            return null;
        }

       // LOGIN (PASSWORD ONLY) + LOOP
       
        private static async Task<string?> LoginWithApiAsync(string username)
        {
            using var http = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };

            while (true)
            {
                Console.WriteLine($"=== Login ({username}) ===");
                Console.Write("Password: ");
                var password = (Console.ReadLine() ?? "").Trim();

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Password is required.\n");
                    continue;
                }

                var body = new { username, password };
                var response = await http.PostAsJsonAsync("/api/auth/login", body);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.Token))
                    {
                        Console.WriteLine("Login failed: no token received from API.\n");
                        continue;
                    }

                    Console.WriteLine("Login successful!\n");
                    return loginResponse.Token;
                }

                if ((int)response.StatusCode == 423)
                {
                    Console.WriteLine("Your account has been locked after too many failed attempts.\n");
                    return null;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Wrong password. Please try again.\n");
                    continue;
                }

                var txt = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login failed: {(int)response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine(txt);
                Console.WriteLine();
            }
        }

        // KEYSHARE
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

        private class LoginResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public int Role { get; set; }
        }
    }
}
