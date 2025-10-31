using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _startRoomMock.Setup(r => r.GetExit(Direction.North)).Returns(_northRoomMock.Object);

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
            deadlyRoomMock.Setup(r => r.IsDeadly).Returns(true);

            _startRoomMock.Setup(r => r.GetExit(Direction.North))
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
            lockedRoomMock.Setup(r => r.RequiresKey).Returns(true);
            _startRoomMock.Setup(r => r.GetExit(Direction.North))
                          .Returns(lockedRoomMock.Object);

            // Player does NOT have a key
            _inventoryMock.Setup(i => i.HasItem(ItemType.Key)).Returns(false);

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
            lockedRoomMock.Setup(r => r.RequiresKey).Returns(true);

            _startRoomMock.Setup(r => r.GetExit(Direction.North))
                          .Returns(lockedRoomMock.Object);

            // Player has a key this time
            _inventoryMock.Setup(i => i.HasItem(ItemType.Key)).Returns(true);

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
            monsterRoomMock.Setup(r => r.HasMonster).Returns(true);
            monsterRoomMock.Setup(r => r.MonsterAlive).Returns(true);

            var nextRoomMock = new Mock<IRoom>();

            monsterRoomMock.Setup(r => r.GetExit(Direction.North))
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
        public void Fight_InMonsterRoom_Sets_MonsterAlive_False()
        {
            // Arrange
            var monsterRoomMock = new Mock<IRoom>();
            monsterRoomMock.Setup(r => r.HasMonster).Returns(true);
            monsterRoomMock.SetupProperty(r => r.MonsterAlive, true);

            _manager = new RoomsManager(monsterRoomMock.Object, _inventoryMock.Object);

            // Act
            _manager.Fight();

            // Assert
            Assert.IsFalse(monsterRoomMock.Object.MonsterAlive,
                "After fighting, the monster should be dead.");
        }

        [TestMethod]
        public void Go_FromMonsterRoom_AfterFight_AllowsLeavingSafely()
        {
            // Arrange
            var nextRoomMock = new Mock<IRoom>();

            var monsterRoomMock = new Mock<IRoom>();
            monsterRoomMock.Setup(r => r.HasMonster).Returns(true);
            monsterRoomMock.SetupProperty(r => r.MonsterAlive, false); // monster is already dead
            monsterRoomMock.Setup(r => r.GetExit(Direction.North)).Returns(nextRoomMock.Object);

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
        public void Fight_InRoomWithoutMonster_DoesNothing()
        {
            // Arrange
            var emptyRoomMock = new Mock<IRoom>();
            emptyRoomMock.Setup(r => r.HasMonster).Returns(false);
            emptyRoomMock.SetupProperty(r => r.MonsterAlive, false);

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
        public void Take_Item_RemovesFromRoom_AndAddsToInventory()
        {
            // Arrange
            var sword = new Item("Sword", ItemType.Sword, "A sharp blade");
            var roomMock = new Mock<IRoom>();
            roomMock.Setup(r => r.GetItems()).Returns(new List<IItem> { sword });
            roomMock.Setup(r => r.TakeItem(sword.Id)).Returns(sword);

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            // Act
            _manager.Take(sword.Id);

            // Assert
            _inventoryMock.Verify(i => i.Add(sword), Times.Once,
                "Item should be added to the inventory");
            roomMock.Verify(r => r.TakeItem(sword.Id), Times.Once,
                "Item should be removed from the room");
        }

        [TestMethod]
        public void Take_NonExistentItem_DoesNothing()
        {
            // Arrange
            var roomMock = new Mock<IRoom>();
            roomMock.Setup(r => r.TakeItem(It.IsAny<string>())).Returns((IItem?)null);

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            // Act
            _manager.Take("nonexistent_item");

            // Assert
            _inventoryMock.Verify(i => i.Add(It.IsAny<IItem>()), Times.Never,
                "No item should be added to inventory");
        }

        [TestMethod]
        public void Look_ShowsRoomDescriptionItemsAndExits()
        {
            // Arrange
            var sword = new Item("Sword", ItemType.Sword, "A sharp blade");
            var roomMock = new Mock<IRoom>();
            roomMock.Setup(r => r.Description).Returns("A dark room");
            roomMock.Setup(r => r.GetItems()).Returns(new List<IItem> { sword });
            roomMock.Setup(r => r.GetExit(Direction.North)).Returns(new Mock<IRoom>().Object);
            roomMock.Setup(r => r.GetExit(Direction.South)).Returns((IRoom?)null);

            _manager = new RoomsManager(roomMock.Object, _inventoryMock.Object);

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            _manager.Look();
            var output = sw.ToString();

            // Assert
            StringAssert.Contains(output, "A dark room");
            StringAssert.Contains(output, "Sword");
            StringAssert.Contains(output, "North");
            Assert.IsFalse(output.Contains("South"), "South should not appear in the exits.");
        }

        [TestMethod]
        public void HasWon_InDoorRoomWithKey_ReturnsTrue()
        {
            // Arrange
            var doorRoomMock = new Mock<IRoom>();
            doorRoomMock.Setup(r => r.RequiresKey).Returns(true);
            _inventoryMock.Setup(i => i.HasItem(ItemType.Key)).Returns(true);

            _manager = new RoomsManager(doorRoomMock.Object, _inventoryMock.Object);

            // Act
            bool won = _manager.HasWon();

            // Assert
            Assert.IsTrue(won, "Player should win when entering the door room with the key.");
        }

        [TestMethod]
        public void HasWon_InDoorRoomWithoutKey_ReturnsFalse()
        {
            // Arrange
            var doorRoomMock = new Mock<IRoom>();
            doorRoomMock.Setup(r => r.RequiresKey).Returns(true);
            _inventoryMock.Setup(i => i.HasItem(ItemType.Key)).Returns(false);

            _manager = new RoomsManager(doorRoomMock.Object, _inventoryMock.Object);

            // Act
            bool won = _manager.HasWon();

            // Assert
            Assert.IsFalse(won, "Player should not win if they are in the door room without the key.");
        }

        [TestMethod]
        public void HasWon_InNormalRoom_ReturnsFalse()
        {
            // Arrange
            var normalRoomMock = new Mock<IRoom>();
            normalRoomMock.Setup(r => r.RequiresKey).Returns(false);

            _manager = new RoomsManager(normalRoomMock.Object, _inventoryMock.Object);

            // Act
            bool won = _manager.HasWon();

            // Assert
            Assert.IsFalse(won, "Player should not win in a normal room.");
        }

    }
}
