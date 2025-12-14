using TestRaiders_TextAdventure;
using TestRaiders_TextAdventure.Core.Interfaces;
using TestRaiders_TextAdventure.Core.Models;

namespace Tests
{
    [TestClass]
    public class GameSetupTests
    {
        private RoomsManager _manager;
        private IRoom _start;
        private IRoom _left;
        private IRoom _right;
        private IRoom _up;
        private IRoom _down;
        private IRoom _deeper;
        private IRoom _sealRoom;
        [TestInitialize]
        public void Setup()
        {
            _manager = GameSetup.InitializeWorld();
            _start = _manager.CurrentRoom;

            _left = _start.GetExit(Direction.West)!;
            _right = _start.GetExit(Direction.East)!;
            _up = _start.GetExit(Direction.North)!;
            _down = _start.GetExit(Direction.South)!;
            _deeper = _down.GetExit(Direction.South)!;
            _sealRoom = _deeper.GetExit(Direction.South)!;
        }

        [TestMethod]
        public void StartRoom_Exists_And_Has_Correct_Name()
        {
            Assert.IsNotNull(_start);
            Assert.AreEqual("Starting room", _start.Name);
        }

        [TestMethod]
        public void StartRoom_Has_All_Expected_Exits()
        {
            Assert.IsNotNull(_start.GetExit(Direction.West), "West (deadly) exit missing");
            Assert.IsNotNull(_start.GetExit(Direction.East), "East (key room) exit missing");
            Assert.IsNotNull(_start.GetExit(Direction.North), "North (locked) exit missing");
            Assert.IsNotNull(_start.GetExit(Direction.South), "South (armory) exit missing");
        }

        [TestMethod]
        public void Rooms_Have_Correct_Flags()
        {
            Assert.IsTrue(_left.IsDeadly, "Left should be deadly.");
            Assert.IsTrue(_up.RequiresKey, "Up should require a key.");
            Assert.IsTrue(_deeper.HasMonster && _deeper.MonsterAlive, "Deeper should contain a living monster.");
            Assert.IsTrue(_sealRoom.RequiresKey && _sealRoom.WinningRoom, "Sealed room should require a key and be the winning room");
        }

        [TestMethod]
        public void Rooms_Have_Correct_Items()
        {
            Assert.IsTrue(_right.GetItems().Any(i => i.Type == ItemType.Key), "Right should contain a key.");
            Assert.IsTrue(_down.GetItems().Any(i => i.Type == ItemType.Sword), "Down should contain a sword.");
            Assert.IsTrue(_deeper.GetItems().Any(i => i.Type == ItemType.Key), "Deeper should contain a key.");
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
    }
}
