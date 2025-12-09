using TestRaiders_TextAdventure;
using TestRaiders_TextAdventure.Core.Interfaces;
using TestRaiders_TextAdventure.Core.Models;

namespace Tests
{
    [TestClass]
    public class RoomTests
    {
        private Room _room;

        [TestInitialize]
        public void Setup()
        {
            _room = new Room("Hall", "A dark hallway.");
        }

        [TestMethod]
        // Checks if the constructor correctly sets properties and defaults
        public void Constructor_Sets_Properties_Correctly()
        {
            Assert.AreEqual("Hall", _room.Name);
            Assert.AreEqual("A dark hallway.", _room.Description);
            Assert.IsFalse(_room.IsDeadly);
            Assert.IsFalse(_room.RequiresKey);
            Assert.IsFalse(_room.HasMonster);
            Assert.IsFalse(_room.WinningRoom);
        }

        [TestMethod]
        public void AddExit_Stores_Room_In_Exits()
        {
            Room nextRoom = new("Library", "Full of books.");
            _room.AddExit(Direction.North, nextRoom);

            Assert.IsTrue(_room.Exits.ContainsKey(Direction.North));
            Assert.AreSame(nextRoom, _room.Exits[Direction.North]);
        }

        [DataTestMethod]
        [DataRow(Direction.North)]
        [DataRow(Direction.East)]
        [DataRow(Direction.South)]
        [DataRow(Direction.West)]
        public void GetExit_Returns_Correct_Room(Direction dir)
        {
            Room nextRoom = new("Library", "Full of books.");
            _room.AddExit(dir, nextRoom);

            IRoom? result = _room.GetExit(dir);

            Assert.AreSame(nextRoom, result);
        }

        [DataTestMethod]
        [DataRow(Direction.North)]
        [DataRow(Direction.East)]
        [DataRow(Direction.South)]
        [DataRow(Direction.West)]
        public void GetExit_Returns_Null_When_No_Exit_Exists(Direction dir)
        {
            IRoom? room = _room.GetExit(dir);
            Assert.IsNull(room);
        }

        [DataTestMethod]
        [DataRow(Direction.North, Direction.South)]
        [DataRow(Direction.East, Direction.West)]
        [DataRow(Direction.South, Direction.North)]
        [DataRow(Direction.West, Direction.East)]
        public void GetExit_Returns_Null_When_Specific_Exit_Doesnt_Exist(Direction dirRoom, Direction dirNone)
        {
            Room nextRoom = new("Library", "Full of books.");
            _room.AddExit(dirRoom, nextRoom);

            IRoom? result = _room.GetExit(dirNone);

            Assert.IsNull(result);
        }

        [TestMethod]
        // Tests if items can be added to the room
        public void AddItem_Adds_Item_To_Room()
        {
            Item item = new("Key", ItemType.Key);
            _room.AddItem(item);

            List<IItem> items = _room.GetItems().ToList();

            Assert.AreEqual(1, items.Count);
            Assert.AreEqual("Key", items[0].Name);
        }

        [TestMethod]
        public void AddItem_Can_Add_Multiple_Items()
        {
            //Arrange
            Item key1 = new("Key of Hope", ItemType.Key);
            Item key2 = new("Key of Despair", ItemType.Key);
            Item sword = new("Sword of Destiny", ItemType.Sword);

            //Act
            _room.AddItem(key1);
            _room.AddItem(key2);
            _room.AddItem(sword);
            List<IItem> items = _room.GetItems().ToList();

            //Assert
            Assert.AreEqual(3, items.Count);
        }

        [TestMethod]
        // Tests if TakeItem removes and returns the correct item
        public void TakeItem_Removes_And_Returns_Item()
        {
            Item item = new("Sword", ItemType.Sword);
            _room.AddItem(item);

            IItem? result = _room.TakeItem(item.Id);

            Assert.AreEqual(item, result);
            Assert.AreEqual(0, _room.GetItems().Count());
        }

        [TestMethod]
        // Tests that TakeItem returns null if the item does not exist
        public void TakeItem_Returns_Null_When_Item_Not_Found()
        {
            IItem? item = _room.TakeItem("does_not_exist");
            Assert.IsNull(item);
        }

        [TestMethod]
        // Tests that the MonsterAlive flag works as expected
        public void MonsterAlive_Can_Be_Updated()
        {
            var monsterRoom = new Room("Basement", "A creepy basement.", hasMonster: true);
            Assert.IsTrue(monsterRoom.MonsterAlive);

            monsterRoom.MonsterAlive = false;
            Assert.IsFalse(monsterRoom.MonsterAlive);
        }

        [TestMethod]
        // Test that a deadly room is correctly marked as deadly
        public void Room_Can_Be_Deadly()
        {
            Room deadlyRoom = new("Trap Room", "Spikes everywhere.", isDeadly: true);

            Assert.IsTrue(deadlyRoom.IsDeadly);
        }

        [TestMethod]
        // Test that a locked room requires a key
        public void Room_Can_Be_Locked()
        {
            var lockedRoom = new Room("Treasure Room", "The door is locked.", requiresKey: true);

            Assert.IsTrue(lockedRoom.RequiresKey);
        }

        [TestMethod]
        // Test that a monster room initializes with MonsterAlive = true
        public void Room_With_Monster_Starts_With_Alive_Monster()
        {
            var monsterRoom = new Room("Cave", "A monster lurks inside.", hasMonster: true);

            Assert.IsTrue(monsterRoom.HasMonster);
            Assert.IsTrue(monsterRoom.MonsterAlive);
        }

        [TestMethod]
        // Test combination: locked and deadly room
        public void Room_Can_Be_Locked_And_Deadly()
        {
            var trickyRoom = new Room("Vault", "A deadly trap behind a locked door.", isDeadly: true, requiresKey: true);

            Assert.IsTrue(trickyRoom.IsDeadly);
            Assert.IsTrue(trickyRoom.RequiresKey);
        }

        [TestMethod]
        // Test combination: deadly monster room
        public void Room_Can_Be_Deadly_And_Have_Monster()
        {
            var bossRoom = new Room("Lair", "The final boss awaits.", isDeadly: true, hasMonster: true);

            Assert.IsTrue(bossRoom.IsDeadly);
            Assert.IsTrue(bossRoom.HasMonster);
            Assert.IsTrue(bossRoom.MonsterAlive);
        }

        [TestMethod]
        // Test full combination: deadly, locked, monster room
        public void Room_Can_Combine_All_Booleans()
        {
            var finalRoom = new Room(
                "End Chamber",
                "A locked, deadly room with a monster guarding the treasure.",
                isDeadly: true,
                requiresKey: true,
                hasMonster: true
            );

            Assert.IsTrue(finalRoom.IsDeadly);
            Assert.IsTrue(finalRoom.RequiresKey);
            Assert.IsTrue(finalRoom.HasMonster);
            Assert.IsTrue(finalRoom.MonsterAlive);
        }

        [TestMethod]
        // Tests the static GenerateDefaultDescription helper
        public void GenerateDefaultDescription_Returns_Correct_Format()
        {
            string result = Room.GenerateDefaultDescription("Hall");
            Assert.AreEqual("description: Hall.", result);
        }

        [TestMethod]
        // Tests that the short constructor chains and generates a default description
        public void Constructor_OnlyName_Generates_Default_Description()
        {
            Room room = new("Dungeon");

            Assert.AreEqual("Dungeon", room.Name);
            Assert.AreEqual("description: Dungeon.", room.Description);
            Assert.IsFalse(room.IsDeadly);
            Assert.IsFalse(room.RequiresKey);
            Assert.IsFalse(room.HasMonster);
        }

        [TestMethod]
        // Tests that the main constructor overrides description properly
        public void Constructor_WithCustomDescription_Does_Not_Use_Default()
        {
            Room room = new("Garden", "A calm, open area with flowers.");

            Assert.AreEqual("Garden", room.Name);
            Assert.AreEqual("A calm, open area with flowers.", room.Description);
            Assert.AreNotEqual(Room.GenerateDefaultDescription("Garden"), room.Description);
        }
    }
}
