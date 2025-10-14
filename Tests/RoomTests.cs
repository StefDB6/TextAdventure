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
    public class RoomTests
    {
        private Room _room;

        [TestInitialize]
        public void Setup()
        {
            _room = new Room("Hall", "A dark hallway.");
        }

        [TestMethod]
        // Checks if the constructor correctly sets properties
        public void Constructor_Sets_Properties_Correctly()
        {
            Assert.AreEqual("Hall", _room.Name);
            Assert.AreEqual("A dark hallway.", _room.Description);
            Assert.IsFalse(_room.IsDeadly);
            Assert.IsFalse(_room.RequiresKey);
            Assert.IsFalse(_room.HasMonster);
        }

								[TestMethod]
								public void AddExit_Stores_Room_In_Exits()
								{
												var nextRoom = new Room("Library", "Full of books.");
												_room.AddExit(Direction.North, nextRoom);

												Assert.IsTrue(_room.Exits.ContainsKey(Direction.North));
												Assert.AreSame(nextRoom, _room.Exits[Direction.North]);
								}

								[TestMethod]
								public void GetExit_Returns_Correct_Room()
								{
												var nextRoom = new Room("Library", "Full of books.");
												_room.AddExit(Direction.East, nextRoom);

												var result = _room.GetExit(Direction.East);

												Assert.AreSame(nextRoom, result);
								}

								[TestMethod]
								public void GetExit_Returns_Null_When_No_Exit_Exists()
								{
												var result = _room.GetExit(Direction.West);
												Assert.IsNull(result);
								}

								[TestMethod]
        // Tests if items can be added to the room
        public void AddItem_Adds_Item_To_Room()
        {
            var item = new Item("Key", "A shiny key");
            _room.AddItem(item);

            var items = _room.GetItems().ToList();

            Assert.AreEqual(1, items.Count);
            Assert.AreEqual("Key", items[0].Name);
        }

        [TestMethod]
        // Tests if TakeItem removes and returns the correct item
        public void TakeItem_Removes_And_Returns_Item()
        {
            var item = new Item("Sword", "A sharp weapon");
            _room.AddItem(item);

            var result = _room.TakeItem(item.Id);

            Assert.AreEqual(item, result);
            Assert.AreEqual(0, _room.GetItems().Count());
        }

        [TestMethod]
        // Tests that TakeItem returns null if the item does not exist
        public void TakeItem_Returns_Null_When_Item_Not_Found()
        {
            var result = _room.TakeItem("does_not_exist");
            Assert.IsNull(result);
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
            var deadlyRoom = new Room("Trap Room", "Spikes everywhere.", isDeadly: true);

            Assert.IsTrue(deadlyRoom.IsDeadly);
            Assert.IsFalse(deadlyRoom.RequiresKey);
            Assert.IsFalse(deadlyRoom.HasMonster);
        }

        [TestMethod]
        // Test that a locked room requires a key
        public void Room_Can_Be_Locked()
        {
            var lockedRoom = new Room("Treasure Room", "The door is locked.", requiresKey: true);

            Assert.IsTrue(lockedRoom.RequiresKey);
            Assert.IsFalse(lockedRoom.IsDeadly);
            Assert.IsFalse(lockedRoom.HasMonster);
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
            Assert.IsFalse(trickyRoom.HasMonster);
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
            var result = Room.GenerateDefaultDescription("Hall");
            Assert.AreEqual("description: Hall.", result);
        }

        [TestMethod]
        // Tests that the short constructor chains and generates a default description
        public void Constructor_OnlyName_Generates_Default_Description()
        {
            var room = new Room("Dungeon");

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
            var room = new Room("Garden", "A calm, open area with flowers.");

            Assert.AreEqual("Garden", room.Name);
            Assert.AreEqual("A calm, open area with flowers.", room.Description);
            Assert.AreNotEqual(Room.GenerateDefaultDescription("Garden"), room.Description);
        }
    }
}
