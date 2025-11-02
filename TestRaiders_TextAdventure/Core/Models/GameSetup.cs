using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class GameSetup
    {
    
        public static RoomsManager InitializeWorld()
        {
            IInventory inventory = new Inventory();

            IRoom start = new Room("Start");
            IRoom left = new Room("Left (deadly)", "A fatal pit awaits here.", isDeadly: true);
            IRoom right = new Room("Right (key room)", "You see something glinting on the floor.");
            IRoom up = new Room("Up (locked door)", "A sturdy wooden door blocks your way.", requiresKey: true);
            IRoom down = new Room("Down (armory)", "An old armory with dusty racks.");
            IRoom deeper = new Room("Deeper (monster lair)", "You hear a growl in the dark...", hasMonster: true);

            // Items
            IItem key = new Item("Key", ItemType.Key, "Opens the locked door.");
            IItem sword = new Item("Sword", ItemType.Sword, "Useful against monsters.");

            right.AddItem(key);    // East contains the key
            down.AddItem(sword);   // South contains the sword

            // Exits from Start
            start.AddExit(Direction.West, left);
            start.AddExit(Direction.East, right);
            start.AddExit(Direction.North, up);
            start.AddExit(Direction.South, down);

            // Optional return paths
            left.AddExit(Direction.East, start);
            right.AddExit(Direction.West, start);
            up.AddExit(Direction.South, start);
            down.AddExit(Direction.North, start);

            // Deeper is below Down
            down.AddExit(Direction.South, deeper);
            deeper.AddExit(Direction.North, down);

            return new RoomsManager(start, inventory);
        }

        public static void RegisterDependencies(ServiceCollection services)
        {
            // Register abstractions to concrete implementations (singleton-style for simplicity)
            services.AddSingleton<IRoomsManager, RoomsManager>();
            services.AddSingleton<IInventory, Inventory>();

           
        }
    }
}
