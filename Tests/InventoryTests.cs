using TestRaiders_TextAdventure;
using TestRaiders_TextAdventure.Core.Interfaces;
using TestRaiders_TextAdventure.Core.Models;

namespace Tests
{
    [TestClass]
    public class InventoryTests
    {
        private readonly Inventory _inventory = new();

        [TestMethod]
        public void Empty_Inventory_Returns_Empty_List()
        {
            List<IItem> items = _inventory.GetAll();
            Assert.AreEqual(items.Count, 0);
        }

        [TestMethod]
        public void AddItem_Adds_One_Item()
        {
            Item item = new("Key", ItemType.Key);
            _inventory.Add(item);
            List<IItem> items = _inventory.GetAll();
            Assert.AreEqual(1, items.Count);
        }

        [TestMethod]
        public void AddItem_Adds_Correct_Item()
        {
            Item item = new("Key to hell", ItemType.Key, "Opens doors");
            _inventory.Add(item);
            List<IItem> items = _inventory.GetAll();
            Assert.AreEqual(items[0].Name, item.Name);
            Assert.AreEqual(items[0].Description, item.Description);
            Assert.AreEqual(items[0].Type, item.Type);
            Assert.AreEqual(items[0].Id, item.Id);
        }

        [TestMethod]
        public void Add_And_HasItem_Works()
        {
            Item item = new("Key", ItemType.Key, "Opens doors");

            _inventory.Add(item);

            Assert.IsTrue(_inventory.HasItem(ItemType.Key));
        }

        [TestMethod]
        public void Remove_Item_Works()
        {
            Item item = new("Sword", ItemType.Sword, "Sharp");

            _inventory.Add(item);
            _inventory.Remove(item);

            Assert.IsFalse(_inventory.HasItem(ItemType.Sword));
        }

        [TestMethod]
        public void HasItem_False_When_Inventory_Empty()
        {
            Assert.IsFalse(_inventory.HasItem(ItemType.Key));
            Assert.IsFalse(_inventory.HasItem(ItemType.Sword));
            Assert.IsFalse(_inventory.HasItem(ItemType.Shield));
        }

        [TestMethod]
        public void HasItem_Distinguishes_Types()
        {
            Item key = new("Key", ItemType.Key);
            Item sword = new("Sword", ItemType.Sword);
            _inventory.Add(key);
            _inventory.Add(sword);

            Assert.IsTrue(_inventory.HasItem(ItemType.Key));
            Assert.IsTrue(_inventory.HasItem(ItemType.Sword));
            Assert.IsFalse(_inventory.HasItem(ItemType.Shield));
        }

        [TestMethod]
        public void Removing_NonExisting_Item_Does_Nothing()
        {
            Item item = new("Key", ItemType.Key);

            // Attempt to remove item not in inventory
            _inventory.Remove(item);

            // Inventory should remain empty, no crash
            Assert.AreEqual(0, _inventory.GetAll().Count);
        }

        [TestMethod]
        public void Inventory_Preserves_InsertionOrder()
        {
            Item key = new("Key", ItemType.Key);
            Item sword = new("Sword", ItemType.Sword);
            Item shield = new("Shield", ItemType.Shield);

            _inventory.Add(key);
            _inventory.Add(sword);
            _inventory.Add(shield);

            List<IItem> items = _inventory.GetAll();
            Assert.AreEqual(key, items[0]);
            Assert.AreEqual(sword, items[1]);
            Assert.AreEqual(shield, items[2]);
        }
    }
}