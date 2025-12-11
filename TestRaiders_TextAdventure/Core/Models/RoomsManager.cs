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

        public RoomsManager(IRoom startingRoom, IInventory inventory)
        {
            _currentRoom = startingRoom;
            _inventory = inventory;
        }

        public string Go(Direction dir)
        {
            var next = _currentRoom.GetExit(dir);
            if (next == null)
                return "There is no exit here.";

            // Deadly trap → Game Over
            if (next.IsDeadly)
            {
                IsGameOver = true;
                return "You fell in a trap! Game Over.";
            }

            // Cannot escape from a living monster
            if (_currentRoom.HasMonster && _currentRoom.MonsterAlive)
            {
                IsGameOver = true;
                return "The monster strikes you down as you try to flee! Game Over.";
            }

            // ---- LOCKED ROOM LOGIC ----
            if (next.RequiresKey)
            {
                // Needs in-game key item
                if (!_inventory.HasItem(ItemType.Key))
                    return "You need a key to access this room.";

                Console.Write("Enter passphrase to decrypt this room: ");
                string? passphrase = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(passphrase))
                    return "Passphrase cannot be empty.";

                // Select correct encrypted file based on room name
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

                // Attempt to decrypt room file
                var decrypted = EncryptedRoomReader.TryDecrypt(file, Keyshare, passphrase);

                if (decrypted == null)
                    return "Incorrect passphrase. The room remains locked.";

                // Decryption successful → show room description
                Console.WriteLine();
                Console.WriteLine("Room decrypted successfully!");
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine(decrypted);
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine();
            }

            // ---- MOVE TO NEXT ROOM ----
            _currentRoom = next;

            if (CheckWin())
            {
                IsGameOver = true;
                return "Congratulations, you won the game!";
            }

            return $"You go {dir}.";
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
    }
}
