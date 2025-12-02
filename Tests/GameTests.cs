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
            Assert.IsTrue(_game._running);
        }

        [TestMethod]
        public void Quit_Works()
        {
            _game.Quit();
            Assert.IsFalse(_game._running);
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
    }
}