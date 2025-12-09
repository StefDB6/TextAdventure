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
            var next = _currentRoom.GetExit(dir);
            if (next == null)
            {
                return "There is no exit here";
            }
            else if (next.IsDeadly)
            {
                IsGameOver = true;
                return "You fell in a trap! Game Over";
            }
            // Check if room is locked and player does NOT have a key
            else if (next.RequiresKey && !_inventory.HasItem(ItemType.Key))
            {
                return "You need a key to access this room";
            }
            // Prevent leaving monster alive
            else if (_currentRoom.HasMonster && _currentRoom.MonsterAlive)
            {
                IsGameOver = true;
                return "The monster hits you before you escape! Game Over";
            }
            else
            {
                // Move to the next room
                _currentRoom = next;

                // Check this here because key is required
                if (CheckWin())
                {
                    return "Congratulations, you won the game!\n";
                }
                return $"Going {dir}";
            }
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
