using TestRaiders_TextAdventure.Core.Models;

namespace TestRaiders_TextAdventure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Configure dependencies
            var services = new ServiceCollection();
            GameSetup.RegisterDependencies(services);

            // Initialise the world of the game
            RoomsManager roomsManager = GameSetup.InitializeWorld();

            // Create the game with that world
            Game game = new(roomsManager);

            // Start the main game loop
            game.Start();
        }
    }
}
