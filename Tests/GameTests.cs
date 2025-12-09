using Moq;
using TestRaiders_TextAdventure;
using TestRaiders_TextAdventure.Core.Interfaces;
using TestRaiders_TextAdventure.Core.Models;

namespace Tests
{
    [TestClass]
    public class GameTests
    {
        private Mock<IRoom> _startRoomMock;
        private Mock<IRoom> _northRoomMock;
        private Mock<IInventory> _inventoryMock;
        private RoomsManager _roomsManager;
        private Game _game;

        [TestInitialize]
        public void Setup()
        {
            _startRoomMock = new();
            _northRoomMock = new();
            _inventoryMock = new();
            _roomsManager = new RoomsManager(_startRoomMock.Object, _inventoryMock.Object);

            _game = new Game(_roomsManager);
        }

        [TestMethod]
        public void Game_Runs_Default()
        {
            Assert.IsTrue(_game.Running);
        }

        [TestMethod]
        public void Quit_Works()
        {
            // New game var because global _game var doesnt update running status
            //Game game = new(_roomsManager);
            _game.Quit();
            Assert.IsFalse(_game.Running);
        }

        [DataTestMethod]
        [DataRow("fake")]
        [DataRow("")]
        // Start function automatically handles null input, converts to empty string
        public void ProcessCommand_Returns_InvalidCommand_On_Bad_Input(string command)
        {
            Assert.AreEqual("Invalid command! Type 'help' to see a list of commands.", _game.ProcessCommand(command));
        }

        [DataTestMethod]
        [DataRow("help", "List of commands")]
        [DataRow("look", "No items in this room")]
        [DataRow("inventory", "You have no items")]
        [DataRow("go n", "There is no exit here")]
        [DataRow("take item_0", "Didnt find matching item in this room!")]
        [DataRow("fight", "There is nothing to fight here")]
        [DataRow("quit", "Closing...")]
        public void ProcessCommand_Returns_Correct_Output(string command, string expectedOutput)
        {
            string output = _game.ProcessCommand(command);
            //Assert.AreEqual(expectedOutput, output);
            Assert.IsTrue(output.Contains(expectedOutput));
        }

        [TestMethod]
        public void Move_Returns_Invalid_Direction_When_Direction_Is_Null()
        {
            Assert.AreEqual("Invalid direction! (n/e/s/w)", _game.Move(null));
        }

        [DataTestMethod]
        [DataRow(Direction.North, "n")]
        [DataRow(Direction.East, "e")]
        [DataRow(Direction.South, "s")]
        [DataRow(Direction.West, "w")]
        public void DirectionString_Gets_Converted_Correctly(Direction correctDirection, string directionStr)
        {
            Direction? direction = _game.GetDirectionFromString(directionStr);
            Assert.AreEqual(correctDirection, direction);
        }

        [DataTestMethod]
        [DataRow("fake")]
        [DataRow("")]
        [DataRow(null)]
        public void NonExistent_Direction_Returns_Null(string directionStr)
        {
            Direction? direction = _game.GetDirectionFromString(directionStr);
            Assert.IsNull(direction);
        }
    }
}