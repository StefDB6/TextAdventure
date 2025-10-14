using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRaiders_TextAdventure.Core.Interfaces
{
    // Represents one location in the text adventure world.
    // A room can hold items, connect to other rooms, and may contain dangers or restrictions.
    public interface IRoom
    {
        // Name of the room (ex. Dungeon, Hallway).
        string Name { get; }

        // Short description shown when the player looks around.
        string Description { get; }

        // If true, entering this room kills the player instantly.
        bool IsDeadly { get; }

        // If true, the room can only be entered with a key.
        bool RequiresKey { get; }

        // If true, the room contains a monster.
        bool HasMonster { get; }

        // Indicates whether the monster is still alive.
        bool MonsterAlive { get; set; }

        // Creates a directional link to another room.
        void AddExit(Direction direction, IRoom room);

        // Returns the room in a given direction, or null if none exists.
        IRoom? GetExit(Direction direction);

        // Adds an item to the room.
        void AddItem(IItem item);

        // Removes and returns an item by ID, or null if not found.
        IItem? TakeItem(string id);

        // Returns all items currently in the room.
        IEnumerable<IItem> GetItems();

        // Helper: generate a default description from the room name
        static string GenerateDefaultDescription(string name) {
            return $"description: {name}.";
								}
    }
}
