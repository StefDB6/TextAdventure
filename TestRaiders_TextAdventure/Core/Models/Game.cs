using Ninject.Infrastructure.Language;
using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    internal class Game : IGame
    {
        //TODO: Remove tight coupling between Game and RoomsManager, now needed for inventory commands
        readonly RoomsManager _roomsManager;
        public bool _running = true;

        public Game(RoomsManager roomsManager)
        {
            _roomsManager = roomsManager;
        }
        public void ProcessCommand(string command)
        {
            string[] splitInput;
            string commandArg = "";

            if (command.Contains("go ") || command.Contains("take "))
            {
                splitInput = command.Split(' ');
                command = splitInput[0];
                commandArg = splitInput[1];
                //Console.WriteLine(commandArg);
            }
            switch (command.ToLower())
            {
                case "help":
                    ShowHelp();
                    break;
                case "look":
                    _roomsManager.Look();
                    break;
                case "inventory":
                    List<IItem> inventoryItems = _roomsManager._inventory.GetAll();
                    Console.WriteLine("Your inventory:");
                    foreach (var item in inventoryItems)
                        Console.WriteLine($"- {item.Name} ({item.Type})");
                    break;
                case "go":
                    Direction? direction = GetDirectionFromString(commandArg);
                    if (direction == null)
                    {
                        Console.WriteLine("Invalid direction! (n/e/s/w)");
                        break;
                    }
                    else
                    {
                        //Console.WriteLine($"Going {direction}");
                        _roomsManager.Go((Direction)direction);
                        break;
                    }
                case "take":
                    _roomsManager.Take(commandArg);
                    break;
                case "fight":
                    _roomsManager.Fight();
                    break;
                case "quit":
                    Quit();
                    Console.WriteLine("Thanks for playing!");
                    break;
                default:
                    Console.WriteLine("Invalid command! Type 'help' to see a list of commands.");
                    break;
            }
            Console.WriteLine($"\nPress enter to continue...");
            Console.ReadLine();
            //Console.Clear();
        }

        public void Quit()
        {
            _running = false;
        }

        public void ShowHelp()
        {
            Console.WriteLine("List of commands:");
            Console.WriteLine($"- look:\t\tShow current room, exits, items, and inventory");
            Console.WriteLine($"- inventory:\tShow inventory");
            Console.WriteLine($"- go n/e/s/w:\tMove in given direction");
            Console.WriteLine($"- take:\t\tpick up item");
            Console.WriteLine($"- fight:\tstart a fight with a monster");
            Console.WriteLine($"- quit:\t\tstop the game");
        }

        public void Start()
        {
            _running = true;
            do
            {
                Console.Write("> ");
                string command = Console.ReadLine() ?? "";
                ProcessCommand(command);
            } while (_running);
        }

        public Direction? GetDirectionFromString(string input)
        {
            switch (input)
            {
                case "n":
                    return Direction.North;
                case "e":
                    return Direction.East;
                case "s":
                    return Direction.South;
                case "w":
                    return Direction.West;
                default:
                    return null;
            }
        }
    }
}
