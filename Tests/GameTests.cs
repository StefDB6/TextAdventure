using Moq;
using TestRaiders_TextAdventure;
using TestRaiders_TextAdventure.Core.Interfaces;
using TestRaiders_TextAdventure.Core.Models;

namespace Tests
{
    [TestClass]
    public class GameTests
    {
        private Room _startRoomMock;
        private Room _northRoomMock;
        private Inventory _inventoryMock;
        private RoomsManager _roomsManager;
        private Game _game;

        [TestInitialize]
        public void Setup()
        {
            _startRoomMock = new("Starter Room");
            _northRoomMock = new("North Room");
            _inventoryMock = new();
            _roomsManager = new RoomsManager(_startRoomMock, _inventoryMock);

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
        [DataRow("n", Direction.North)]
        [DataRow("e", Direction.East)]
        [DataRow("s", Direction.South)]
        [DataRow("w", Direction.West)]
        public void DirectionString_Gets_Converted_Correctly(string directionStr, Direction correctDirection)
        {
            Direction? direction = _game.GetDirectionFromString(directionStr);
            Assert.AreEqual(correctDirection, direction);
        }
    }
}
