using System.Net.Http.Headers;
using System.Net.Http.Json;
using TestRaiders_TextAdventure.Core.Encryption;
using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class RoomsManager : IRoomsManager
    {
        private IRoom _currentRoom;
        public readonly IInventory _inventory;

        public IRoom CurrentRoom => _currentRoom;

        public bool IsGameOver { get; private set; }

        public string Keyshare { get; set; } = "";
        public string JwtToken { get; set; } = "";
        public bool IsAdmin { get; private set; }

        public RoomsManager(IRoom startingRoom, IInventory inventory)
        {
            _currentRoom = startingRoom;
            _inventory = inventory;
        }

        public string Go(Direction dir)
        {
            var next = _currentRoom.GetExit(dir);

            // Topology is absolute
            if (next == null)
                return "There is no exit here.";

            // Deadly room
            if (next.IsDeadly && !IsAdmin)
            {
                IsGameOver = true;
                return "You fell in a trap! Game Over.";
            }

            // Monster prevents escape
            if (_currentRoom.HasMonster && _currentRoom.MonsterAlive && !IsAdmin)
            {
                IsGameOver = true;
                return "The monster strikes you down as you try to flee! Game Over.";
            }

            // Locked room logic
            if (next.RequiresKey && !IsAdmin)
            {
                if (!_inventory.HasItem(ItemType.Key))
                    return "You need a key to access this room.";

                string passphrase = "coolpasswoord";

                string? file = next.Name switch
                {
                    string name when name.Contains("Throne", StringComparison.OrdinalIgnoreCase)
                        => "throne.enc",

                    string name when name.Contains("Seal", StringComparison.OrdinalIgnoreCase)
                        => "seal.enc",

                    _ => null
                };

                if (file == null)
                    return "ERROR: Unknown encrypted room file.";

                var decrypted = EncryptedRoomReader.TryDecrypt(file, Keyshare, passphrase);

                if (decrypted == null)
                    return "Incorrect passphrase. The room remains locked.";

                Console.WriteLine();
                Console.WriteLine("Room decrypted successfully!");
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine(decrypted);
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine();
            }

            if (IsAdmin)
            {
                if (next.RequiresKey)
                    Console.WriteLine("[ADMIN] Lock bypassed.");

                if (next.IsDeadly)
                    Console.WriteLine("[ADMIN] Deadly room ignored.");

                if (_currentRoom.HasMonster && _currentRoom.MonsterAlive)
                    Console.WriteLine("[ADMIN] Monster ignored.");
            }

            _currentRoom = next;

            if (CheckWin())
            {
                IsGameOver = true;
                return "Congratulations, you won the game!";
            }

            return IsAdmin
                ? $"You phase through restrictions and go {dir}."
                : $"You go {dir}.";
        }


        public string Look()
        {
            List<string> output = [
                _currentRoom.Description
            ];

            var items = _currentRoom.GetItems();
            if (items is null || items.Count == 0)
            {
                output.Add("No items in this room.");
            }
            else
            {
                output.Add("Items in the room:");
                foreach (var item in items)
                    output.Add(item.ToString());
            }

            // Show available exits
            var directions = Enum.GetValues(typeof(Direction))
                                 .Cast<Direction>()
                                 .Where(d => _currentRoom.GetExit(d) != null)
                                 .ToList();

            if (directions.Count != 0)
            {
                output.Add($"Exits available: {string.Join(", ", directions)}");
            }
            else
            {
                output.Add("No exits available.");
            }
            return String.Join("\n", output);
        }

        public string Take(string? itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return "Please give an item Id!";
            }
            else
            {
                // Try to take the item from the current room
                IItem? item = _currentRoom.TakeItem(itemId);
                if (item != null)
                {
                    _inventory.Add(item);
                    return $"You picked up: {item.Name}";
                }
                return "Didnt find matching item in this room!";
            }
        }

        public string Fight()
        {
            if (_currentRoom.HasMonster && _currentRoom.MonsterAlive)
            {
                _currentRoom.MonsterAlive = false;
                return "You have defeated the monster!";
            }
            else
            {
                return "There is nothing to fight here.";
            }
        }
        public bool CheckWin()
        {
            return _currentRoom.WinningRoom;
        }

        public async Task LoadPlayerRoleAsync(string apiBaseUrl)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseUrl);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtToken);

            var response = await client.GetAsync("/api/auth/me");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to retrieve player role.");

            var me = await response.Content.ReadFromJsonAsync<AuthMeResponse>();

            IsAdmin = string.Equals(me?.Role, "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
