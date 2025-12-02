using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    internal class Game : IGame
    {
        //TODO: Remove tight coupling between Game and RoomsManager, now needed for inventory commands
        readonly RoomsManager _roomsManager;
        public bool _running = true;
        private bool gameOver = false;

        public Game(RoomsManager roomsManager)
        {
            _roomsManager = roomsManager;
        }

        public void Start()
        {
            _running = true;
            do
            {
                Console.Write("> ");
                string command = Console.ReadLine() ?? "";
                ProcessCommand(command);
                if (_roomsManager.IsGameOver)
                {
                    _running = false;
                }
            } while (_running);

            Console.WriteLine("Thanks for playing! (press enter to continue)");
            Console.ReadLine();
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
                    Console.WriteLine(ShowHelp());
                    break;
                case "look":
                    Console.WriteLine(_roomsManager.Look());
                    break;
                case "inventory":
                    Console.WriteLine(ShowInventory());
                    break;
                case "go":
                    Console.WriteLine(Move(commandArg));
                    break;
                case "take":
                    Console.WriteLine(TakeItem());
                    break;
                case "fight":
                    _roomsManager.Fight();
                    break;
                case "quit":
                    Quit();
                    break;
                default:
                    Console.WriteLine("Invalid command! Type 'help' to see a list of commands.");
                    break;
            }
            //Console.WriteLine($"\nPress enter to continue...");
            //Console.ReadLine();
            //Console.Clear();
        }

        public void Quit()
        {
            _running = false;
        }

        public string ShowHelp()
        {
            string[] helpList =
            [
                "List of commands",
                "look:\t\tShow current room, exits, items, and inventory",
                "inventory:\tShow inventory",
                "go n/e/s/w:\tMove in given direction",
                "take:\t\tpick up item",
                "fight:\tstart a fight with a monster",
                "quit:\t\tstop the game"
            ];
            // Format list of commands
            return String.Join("\n- ", helpList);
        }

        public string ShowInventory()
        {
            List<string> output =
            [
                "Your inventory:",
            ];
            List<IItem> inventoryItems = _roomsManager._inventory.GetAll();
            foreach (var item in inventoryItems)
                output.Add($"{item.Name} ({item.Type})");
            return String.Join("\n- ", output);
        }

        public string Move(string dir)
        {
            Direction? direction = GetDirectionFromString(dir);
            if (direction == null)
            {
                return "Invalid direction! (n/e/s/w)";
            }
            else
            {
                //Console.WriteLine($"Going {direction}");
                return _roomsManager.Go((Direction)direction);
            }
        }

        public string TakeItem()
        {
            List<IItem> roomItems = (List<IItem>)_roomsManager.CurrentRoom.GetItems();
            string? itemId = null;
            if (!(roomItems.Count == 0))
            {
                itemId = roomItems.First().Id;
            }

            return _roomsManager.Take(itemId);
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