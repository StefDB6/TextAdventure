using TestRaiders_TextAdventure.Core.Models;

namespace TestRaiders_TextAdventure
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var services = new ServiceCollection();
            GameSetup.RegisterDependencies(services);

            // 1) Initialise the world of the game
            RoomsManager roomsManager = GameSetup.InitializeWorld();

            // 2) Create the game with that world
            Game game = new(roomsManager);

            // 3) Start the game loop
            game.Start();
        }
    }
}
