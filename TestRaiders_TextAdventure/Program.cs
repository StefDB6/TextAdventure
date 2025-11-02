using TestRaiders_TextAdventure.Core.Interfaces;
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
            var roomsManager = GameSetup.InitializeWorld();

            // 2) Create the game with that world
            var game = new Game(roomsManager);


            // 3) Start the game loop
            Console.WriteLine("Welcome to TestRaiders! Type 'help' for commands.");
            game.Start();

            Console.WriteLine("Game exited. Press any key to close...");
        }
    }
}
