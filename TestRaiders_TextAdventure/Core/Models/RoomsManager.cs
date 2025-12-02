using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class RoomsManager : IRoomsManager
    {
        private IRoom _currentRoom;
        public readonly IInventory _inventory;

        public IRoom CurrentRoom => _currentRoom;

        public bool IsGameOver { get; private set; }

        public RoomsManager(IRoom startingRoom, IInventory inventory)
        {
            _currentRoom = startingRoom;
            _inventory = inventory;
        }

        public string Go(Direction dir)
        {
            string output = "";
            var next = _currentRoom.GetExit(dir);
            if (_currentRoom.IsDeadly)
            {
                // TODO: Deadly rooms only trigger when moving out instead of in
                output = "You fell in a trap!";
                IsGameOver = true;
            }
            else if (next == null)
            {
                output = "There is no exit here";
            }
            // Check if room is locked and player does NOT have a key
            else if (next.RequiresKey && !_inventory.HasItem(ItemType.Key))
            {
                output = "You need a key to access this room";
            }
            // Prevent leaving monster alive
            else if (_currentRoom.HasMonster && _currentRoom.MonsterAlive)
            {
                output = "The monster blocks your way";
                //IsGameOver = true;
            }
            else
            {
                output = $"Going {dir}";
                // Move to the next room
                _currentRoom = next;
            }
            return output;
        }

        public string Look()
        {
            List<string> output = [
                _currentRoom.Description
            ];

            var items = _currentRoom.GetItems();
            if (items.Any())
            {
                output.Add("Items in the room:");
                foreach (var item in items)
                    output.Add($"- {item.Name} ({item.Type})");
            }
            else
            {
                output.Add("No items in this room.");
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
            string output = "";
            if (string.IsNullOrWhiteSpace(itemId))
                output = "There is no item in this room";

            // Try to take the item from the current room
            // NOTE: this will always succeed as GetItems gets called to call this function in Game.cs
            IItem? item = _currentRoom.TakeItem(itemId);

            if (item != null)
            {
                _inventory.Add(item);
                output = $"You picked up: {item.Name}";
            }
            return output;
        }

        public void Fight()
        {
            if (_currentRoom.HasMonster && _currentRoom.MonsterAlive)
            {
                _currentRoom.MonsterAlive = false;
                Console.WriteLine("You have defeated the monster!");
            }
            else
            {
                Console.WriteLine("There is nothing to fight here.");
            }
        }
        public bool HasWon()
        {
            // The player wins if:
            // 1. The current room requires a key (the “door” room)
            // 2. The player has a key in their inventory
            return _currentRoom.RequiresKey && _inventory.HasItem(ItemType.Key);
        }
    }
}
