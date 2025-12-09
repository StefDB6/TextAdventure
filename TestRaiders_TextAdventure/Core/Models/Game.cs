using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    internal class Game : IGame
    {
        //TODO: Remove tight coupling between Game and RoomsManager, now needed for inventory commands
        private readonly RoomsManager _roomsManager;
        public bool Running = true;

        public Game(RoomsManager roomsManager)
        {
            _roomsManager = roomsManager;
        }

        public void Start()
        {
            Console.WriteLine("Welcome to TestRaiders! Type 'help' for commands.");
            Running = true;

            // Main game loop: get command > run functions > show result
            do
            {
                Console.Write("> ");
                string command = Console.ReadLine() ?? "";
                Console.WriteLine(ProcessCommand(command));
                if (_roomsManager.IsGameOver || _roomsManager.CheckWin())
                {
                    Running = false;
                }
            } while (Running);

            Console.WriteLine("Thanks for playing! (press enter to continue)");
            Console.ReadLine();
        }

        public string ProcessCommand(string command)
        {
            string[] splitInput;
            string commandArg = "";

            if (command.Contains("go ") || command.Contains("take "))
            {
                splitInput = command.Split(' ');
                command = splitInput[0];
                commandArg = splitInput[1];
            }

            switch (command.ToLower())
            {
                case "help":
                    return ShowHelp();
                case "look":
                    return _roomsManager.Look();
                case "inventory":
                    return ShowInventory();
                case "go":
                    return Move(commandArg);
                case "take":
                    return _roomsManager.Take(commandArg);
                case "fight":
                    return _roomsManager.Fight();
                case "quit":
                    Quit();
                    return "Closing...";
                default:
                    return "Invalid command! Type 'help' to see a list of commands.";
            }
        }

        public void Quit()
        {
            Running = false;
        }

        public string ShowHelp()
        {
            string[] helpList =
            [
                "List of commands",
                "look:\t\tShow current room, exits, items, and inventory",
                "inventory:\tShow inventory",
                "go n/e/s/w:\tMove in given direction",
                "take item_id:\tPick up item with its Id",
                "fight:\tStart a fight with a monster",
                "quit:\t\tStop the game"
            ];
            // Format list of commands
            return String.Join("\n- ", helpList);
        }

        public string ShowInventory()
        {
            List<string> output = ["Your inventory:"];
            List<IItem> inventoryItems = _roomsManager._inventory.GetAll();
            if (inventoryItems is null)
            {
                return "You have no items in your inventory!";
            }
            foreach (var item in inventoryItems)
            {
                output.Add(item.ToString());
            }

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
                return _roomsManager.Go(direction.Value);
            }
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