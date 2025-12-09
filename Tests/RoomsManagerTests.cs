using Moq;
using TestRaiders_TextAdventure;
using TestRaiders_TextAdventure.Core.Interfaces;
using TestRaiders_TextAdventure.Core.Models;

namespace Tests
{
    [TestClass]
    public class RoomsManagerTests
    {

        private Mock<IRoom> _startRoomMock;
        private Mock<IRoom> _northRoomMock;
        private Mock<IInventory> _inventoryMock;
        private RoomsManager _manager;

        [TestInitialize]
        public void Setup()
        {
            _startRoomMock = new Mock<IRoom>();
            _northRoomMock = new Mock<IRoom>();
            _inventoryMock = new Mock<IInventory>();

            // When moving north, return the next room
            _startRoomMock.Setup(room => room.GetExit(Direction.North)).Returns(_northRoomMock.Object);

            _manager = new RoomsManager(_startRoomMock.Object, _inventoryMock.Object);
        }

        [TestMethod]
        public void Go_North_Changes_CurrentRoom_To_ConnectedRoom()
        {
            _manager.Go(Direction.North);
            Assert.AreEqual(_northRoomMock.Object, _manager.CurrentRoom);
        }

        [TestMethod]
        public void Go_NoExit_Keeps_CurrentRoom_Unchanged()
        {
            // Arrange
            var initialRoom = _manager.CurrentRoom;

            // Act
            _manager.Go(Direction.South); // South is not set up — should stay in place

            // Assert
            Assert.AreEqual(initialRoom, _manager.CurrentRoom,
                "If no exit exists, player should remain in the same room.");
        }


        [TestMethod]
        public void Go_IntoDeadlyRoom_Sets_GameOver_ToTrue()
        {
            // Arrange
            var deadlyRoomMock = new Mock<IRoom>();
            deadlyRoomMock.Setup(room => room.IsDeadly).Returns(true);

            _startRoomMock.Setup(room => room.GetExit(Direction.North))
                          .Returns(deadlyRoomMock.Object);

            // Act
            _manager.Go(Direction.North);

            // Assert
            Assert.IsTrue(_manager.IsGameOver,
                "Entering a deadly room should set IsGameOver to true.");
        }

        [TestMethod]
        public void Go_ToLockedRoom_WithoutKey_DoesNotChangeRoom()
        {
            // Arrange
            var lockedRoomMock = new Mock<IRoom>();
            lockedRoomMock.Setup(room => room.RequiresKey).Returns(true);
            _startRoomMock.Setup(room => room.GetExit(Direction.North))
                          .Returns(lockedRoomMock.Object);

            // Player does NOT have a key
            _inventoryMock.Setup(inv => inv.HasItem(ItemType.Key)).Returns(false);

            var initialRoom = _startRoomMock.Object;

            // Act
            _manager.Go(Direction.North);

            // Assert
            Assert.AreSame(initialRoom, _manager.CurrentRoom,
                "Player should not enter locked room without a key.");
            Assert.IsFalse(_manager.IsGameOver,
                "Game should not end when trying to enter a locked room without key.");
        }

        [TestMethod]
        public void Go_ToLockedRoom_WithKey_ChangesRoom()
        {
            // Arrange
            var lockedRoomMock = new Mock<IRoom>();
            lockedRoomMock.Setup(room => room.RequiresKey).Returns(true);

            _startRoomMock.Setup(room => room.GetExit(Direction.North))
                          .Returns(lockedRoomMock.Object);

            // Player has a key this time
            _inventoryMock.Setup(inv => inv.HasItem(ItemType.Key)).Returns(true);

            // Act
            _manager.Go(Direction.North);

            // Assert
            Assert.AreEqual(lockedRoomMock.Object, _manager.CurrentRoom,
                "Player should be able to enter locked room when they have a key.");
            Assert.IsFalse(_manager.IsGameOver,
                "Game should not end when entering a locked room with a key.");
        }

        [TestMethod]
        public void Go_FromMonsterRoom_WhileMonsterAlive_SetsGameOver()
        {
            // Arrange
            var monsterRoomMock = new Mock<IRoom>();
            monsterRoomMock.Setup(room => room.HasMonster).Returns(true);
            monsterRoomMock.Setup(room => room.MonsterAlive).Returns(true);

            var nextRoomMock = new Mock<IRoom>();

            monsterRoomMock.Setup(room => room.GetExit(Direction.North))
                           .Returns(nextRoomMock.Object);

            // Place the player in the monster room
            _manager = new RoomsManager(monsterRoomMock.Object, _inventoryMock.Object);

            // Act
            _manager.Go(Direction.North);

            // Assert
            Assert.IsTrue(_manager.IsGameOver,
                "Leaving a monster room while the monster is alive should end the game.");
        }

        [TestMethod]
        public void Go_FromMonsterRoom_AfterFight_AllowsLeavingSafely()
        {
            // Arrange
            var nextRoomMock = new Mock<IRoom>();

            var monsterRoomMock = new Mock<IRoom>();
            monsterRoomMock.Setup(room => room.HasMonster).Returns(true);
            monsterRoomMock.SetupProperty(room => room.MonsterAlive, false); // monster is already dead
            monsterRoomMock.Setup(room => room.GetExit(Direction.North)).Returns(nextRoomMock.Object);

            _manager = new RoomsManager(monsterRoomMock.Object, _inventoryMock.Object);

            // Act
            _manager.Go(Direction.North);

            // Assert
            Assert.AreEqual(nextRoomMock.Object, _manager.CurrentRoom,
                "Player should be able to leave monster room safely after fighting.");
            Assert.IsFalse(_manager.IsGameOver,
                "Game should not end after leaving a monster room when monster is dead.");
        }

        [TestMethod]
        public void Look_Shows_Room_Description()
        {
            // Arrange
            var roomMock = new Mock<IRoom>();
            roomMock.Setup(room => room.Description).Returns("A dark room");

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            // Act
            string output = _manager.Look();

            // Assert
            StringAssert.Contains(output, "A dark room");
        }

        [TestMethod]
        public void Look_Shows_Room_Items()
        {
            // Arrange
            var sword = new Item("Sword of Destiny", ItemType.Sword, "A sharp blade");
            var roomMock = new Mock<IRoom>();
            roomMock.Setup(room => room.GetItems()).Returns(new List<IItem> { sword });

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            // Act
            string output = _manager.Look();

            // Assert
            StringAssert.Contains(output, sword.Name);
            StringAssert.Contains(output, sword.Description);
        }

        [DataTestMethod]
        [DataRow(Direction.North)]
        [DataRow(Direction.East)]
        [DataRow(Direction.South)]
        [DataRow(Direction.West)]
        public void Look_Shows_Room_Exits(Direction dir)
        {
            // Arrange
            var roomMock = new Mock<IRoom>();
            roomMock.Setup(room => room.GetExit(dir)).Returns(new Mock<IRoom>().Object);

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            // Act
            string output = _manager.Look();

            // Assert
            StringAssert.Contains(output, dir.ToString());
        }

        [TestMethod]
        public void Look_Shows_No_Items_In_Room()
        {
            var roomMock = new Mock<IRoom>();

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            string output = _manager.Look();

            StringAssert.Contains(output, "No items in this room.", "No items were added");
        }

        [TestMethod]
        public void Look_Shows_No_Exits_Available()
        {
            var roomMock = new Mock<IRoom>();

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            string output = _manager.Look();

            StringAssert.Contains(output, "No exits available.", "No exits were added");
        }

        [TestMethod]
        public void Take_Item_RemovesFromRoom_AndAddsToInventory()
        {
            // Arrange
            var sword = new Item("Sword", ItemType.Sword, "A sharp blade");
            var roomMock = new Mock<IRoom>();
            roomMock.Setup(room => room.GetItems()).Returns(new List<IItem> { sword });
            roomMock.Setup(room => room.TakeItem(sword.Id)).Returns(sword);

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            // Act
            _manager.Take(sword.Id);

            // Assert
            _inventoryMock.Verify(inv => inv.Add(sword), Times.Once,
                "Item should be added to the inventory");
            roomMock.Verify(room => room.TakeItem(sword.Id), Times.Once,
                "Item should be removed from the room");
        }

        [TestMethod]
        public void Take_NonExistentItem_DoesNothing()
        {
            // Arrange
            var roomMock = new Mock<IRoom>();
            roomMock.Setup(room => room.TakeItem(It.IsAny<string>())).Returns((IItem?)null);

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            // Act
            _manager.Take("nonexistent_item");

            // Assert
            _inventoryMock.Verify(inv => inv.Add(It.IsAny<IItem>()), Times.Never,
                "No item should be added to inventory");
        }

        [TestMethod]
        public void Fight_InMonsterRoom_Sets_MonsterAlive_False()
        {
            // Arrange
            var monsterRoomMock = new Mock<IRoom>();
            monsterRoomMock.Setup(room => room.HasMonster).Returns(true);
            monsterRoomMock.SetupProperty(room => room.MonsterAlive, true);

            _manager = new RoomsManager(monsterRoomMock.Object, _inventoryMock.Object);

            // Act
            _manager.Fight();

            // Assert
            Assert.IsFalse(monsterRoomMock.Object.MonsterAlive,
                "After fighting, the monster should be dead.");
        }

        [TestMethod]
        public void Fight_InRoomWithoutMonster_DoesNothing()
        {
            // Arrange
            var emptyRoomMock = new Mock<IRoom>();
            emptyRoomMock.Setup(room => room.HasMonster).Returns(false);
            emptyRoomMock.SetupProperty(room => room.MonsterAlive, false);

            _manager = new RoomsManager(emptyRoomMock.Object, _inventoryMock.Object);

            // Act
            _manager.Fight();

            // Assert
            Assert.IsFalse(emptyRoomMock.Object.MonsterAlive,
                "MonsterAlive should remain false when no monster is present.");
            Assert.IsFalse(_manager.IsGameOver,
                "Game should not end when fighting in a room without a monster.");
        }

        [TestMethod]
        public void HasWon_InNormalRoom_ReturnsFalse()
        {
            // Arrange
            var normalRoomMock = new Mock<IRoom>();
            normalRoomMock.Setup(room => room.WinningRoom).Returns(false);

            _manager = new RoomsManager(normalRoomMock.Object, _inventoryMock.Object);

            // Act
            bool won = _manager.CheckWin();

            // Assert
            Assert.IsFalse(won, "Player should not win in a normal room.");
        }

        [TestMethod]
        public void HasWon_InWinningRoom_ReturnsTrue()
        {
            // Arrange
            var winningRoomMock = new Mock<IRoom>();
            winningRoomMock.Setup(room => room.WinningRoom).Returns(true);

            _manager = new RoomsManager(winningRoomMock.Object, _inventoryMock.Object);

            // Act
            bool won = _manager.CheckWin();

            // Assert
            Assert.IsTrue(won, "Player should win in the winning room.");
        }

    }
}
