using TestRaiders_TextAdventure.Core.Models;

namespace TestRaiders_TextAdventure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Test");
            Console.WriteLine("item");

            // Just for testing, final program should only have bottom 2 lines here, maybe only last
            Room startRoom = new("startRoom");
            Room northroom = new("northRoom");
            Inventory inventory = new();
            RoomsManager manager = new(startRoom, inventory);

            Game game = new(manager);
            game.Start();
        }
    }
}
