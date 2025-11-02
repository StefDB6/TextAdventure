using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void Go(Direction dir)
        {
            var next = _currentRoom.GetExit(dir);
            if (next == null)
                return;

            // Check if room is locked and player does NOT have a key
            if (next.RequiresKey && !_inventory.HasItem(ItemType.Key))
                return;

            // Prevent leaving monster alive
            if (_currentRoom.HasMonster && _currentRoom.MonsterAlive)
            {
                IsGameOver = true;
                return;
            }

            // Move to the next room
            _currentRoom = next;

            if (_currentRoom.IsDeadly)
                IsGameOver = true;
        }

        public void Look()
        {
            Console.WriteLine($"You are in: {_currentRoom.Description}");

            var items = _currentRoom.GetItems();
            if (items.Any())
            {
                Console.WriteLine("Items in the room:");
                foreach (var item in items)
                    Console.WriteLine($"- {item.Name} ({item.Type})");
            }
            else
            {
                Console.WriteLine("No items in this room.");
            }

            // Show available exits
            var directions = Enum.GetValues(typeof(Direction))
                                 .Cast<Direction>()
                                 .Where(d => _currentRoom.GetExit(d) != null)
                                 .ToList();

            if (directions.Any())
            {
                Console.WriteLine("Exits available: " + string.Join(", ", directions));
            }
            else
            {
                Console.WriteLine("No exits available.");
            }
        }

        public void Take(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return;

            // Try to take the item from the current room
            IItem? item = _currentRoom.TakeItem(itemId);

            if (item != null)
            {
                _inventory.Add(item);

                Console.WriteLine($"You picked up: {item.Name}");
            }
            else
            {
                Console.WriteLine("There is no such item here.");
            }
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
