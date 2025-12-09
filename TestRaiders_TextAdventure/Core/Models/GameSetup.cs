using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class GameSetup
    {

        public static RoomsManager InitializeWorld()
        {
            IInventory inventory = new Inventory();

            // Rooms
            Room start = new("Starting room");
            Room left = new("Left (deadly)", "A fatal pit awaits here.", isDeadly: true);
            Room right = new("Right (key room)", "You see something glinting on the floor.");
            Room up = new("Up (locked door)", "The throne awaits you!", requiresKey: true, winningRoom: true);
            Room down = new("Down (armory)", "An old armory with dusty weapon racks.");
            Room deeper = new("Deeper (monster lair)", "A dark cave with a dangerous creature...", hasMonster: true);

            // Items
            Item key = new("Key of Success", ItemType.Key, "Opens the way forward.");
            Item sword = new("Sword of Destiny", ItemType.Sword, "Useful against monsters.");

            // Add Items to Rooms
            right.AddItem(key);    // East contains the key
            down.AddItem(sword);   // South contains the sword

            // Exits from start
            start.AddExit(Direction.West, left);
            start.AddExit(Direction.East, right);
            start.AddExit(Direction.North, up);
            start.AddExit(Direction.South, down);

            // Return paths
            left.AddExit(Direction.East, start);
            right.AddExit(Direction.West, start); // (Not needed in practice because West kills instantly)
            up.AddExit(Direction.South, start);
            down.AddExit(Direction.North, start);

            // Connect 2 south rooms
            down.AddExit(Direction.South, deeper);
            deeper.AddExit(Direction.North, down);

            return new RoomsManager(start, inventory);
        }

        public static void RegisterDependencies(ServiceCollection services)
        {
            // Register abstractions to concrete implementations
            services.AddSingleton<IRoomsManager, RoomsManager>();
            services.AddSingleton<IInventory, Inventory>();
        }
    }
}
