using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class GameSetup
    {

        public static RoomsManager InitializeWorld()
        {
            IInventory inventory = new Inventory();

            // --- ROOMS ---
            Room start = new("Starting room");

            Room left = new("Left",
                "A fatal pit awaits here.",
                isDeadly: true);

            Room right = new("Right",
                "You see something glinting on the floor.");

            Room up = new("Throne Room",
                "A majestic throne stands before you.",
                requiresKey: true);

            Room down = new("Armory",
                "An old armory with dusty weapon racks.");

            Room deeper = new("Deeper",
                "A dark cave with a dangerous creature...",
                hasMonster: true);

            Room sealRoom = new("Seal Room",
                "A mysterious ancient chamber sealed by magic.",
                requiresKey: true, winningRoom: true);

            // --- ITEMS ---
            Item keyA = new("Key A", ItemType.Key, "Opens the first locked door.");
            Item keyB = new("Key B", ItemType.Key, "Opens the second locked door.");
            Item sword = new("Sword of Destiny", ItemType.Sword, "Useful against monsters.");

            // --- PLACE ITEMS ---
            right.AddItem(keyA);     // Key A found in right
            deeper.AddItem(keyB);    // Key B found deeper after monster
            down.AddItem(sword);     // optional sword pickup

            // --- EXITS FROM START ---
            start.AddExit(Direction.West, left);
            start.AddExit(Direction.East, right);
            start.AddExit(Direction.North, up);
            start.AddExit(Direction.South, down);

            // --- RETURN PATHS ---
            left.AddExit(Direction.East, start);
            right.AddExit(Direction.West, start);
            up.AddExit(Direction.South, start);
            down.AddExit(Direction.North, start);

            // --- DEEP ZONE ---
            down.AddExit(Direction.South, deeper);
            deeper.AddExit(Direction.North, down);

            // --- SECOND LOCKED ROOM ---
            deeper.AddExit(Direction.South, sealRoom);
            sealRoom.AddExit(Direction.North, deeper);

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
