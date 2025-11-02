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
    public class GameSetupTests
    {
        [TestMethod]
        public void InitializeWorld_StartRoom_Has_All_Expected_Exits()
        {
            var manager = GameSetup.InitializeWorld();
            var start = manager.CurrentRoom;

            Assert.IsNotNull(start);
            Assert.AreEqual("Start", start.Name);

            Assert.IsNotNull(start.GetExit(Direction.West), "West (deadly) exit missing");
            Assert.IsNotNull(start.GetExit(Direction.East), "East (key room) exit missing");
            Assert.IsNotNull(start.GetExit(Direction.North), "North (locked) exit missing");
            Assert.IsNotNull(start.GetExit(Direction.South), "South (armory) exit missing");
        }

        [TestMethod]
        public void InitializeWorld_Rooms_Have_Correct_Flags_And_Items()
        {
            var manager = GameSetup.InitializeWorld();

            var start = manager.CurrentRoom;
            var left = start.GetExit(Direction.West)!;
            var right = start.GetExit(Direction.East)!;
            var up = start.GetExit(Direction.North)!;
            var down = start.GetExit(Direction.South)!;
            var deeper = down.GetExit(Direction.South)!;

            Assert.IsTrue(left.IsDeadly, "Left should be deadly.");
            Assert.IsTrue(up.RequiresKey, "Up should require a key.");
            Assert.IsTrue(deeper.HasMonster && deeper.MonsterAlive, "Deeper should contain a living monster.");

            Assert.IsTrue(right.GetItems().Any(i => i.Type == ItemType.Key), "Right should contain a key.");
            Assert.IsTrue(down.GetItems().Any(i => i.Type == ItemType.Sword), "Down should contain a sword.");
        }

        [TestMethod]
        public void RegisterDependencies_Registers_Core_Services()
        {
            var services = new ServiceCollection();

            GameSetup.RegisterDependencies(services);

            Assert.IsTrue(services.IsRegistered<IRoomsManager, RoomsManager>(),
                "IRoomsManager should be registered to RoomsManager.");
            Assert.IsTrue(services.IsRegistered<IInventory, Inventory>(),
                "IInventory should be registered to Inventory.");
        }

        //Can my service collection effectively create a registered service without crashing or nullifying it?
        [TestMethod]
        public void Resolve_SmokeTest_For_Registered_Service()
        {
            var services = new ServiceCollection();
            GameSetup.RegisterDependencies(services);
        }
    }
}
