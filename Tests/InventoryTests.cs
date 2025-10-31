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
    public class InventoryTests
    {
        [TestMethod]
        public void Empty_Inventory_Returns_Empty_List()
        {
            Inventory inventory = new();
            var test = inventory.GetAll();
            Assert.AreEqual(test.Count, 0);
        }

        [TestMethod]
        public void AddItem_Adds_One_Item()
        {
            Inventory inventory = new();
            Item item = new("Key", ItemType.Key);
            inventory.Add(item);
            var test = inventory.GetAll();
            Assert.AreEqual(1, test.Count);
        }

        [TestMethod]
        public void AddItem_Adds_Correct_Item()
        {
            Inventory inventory = new();
            Item item = new("Key to hell", ItemType.Key, "Opens doors");
            inventory.Add(item);
            List<IItem> inventoryList = inventory.GetAll();
            Assert.AreEqual(inventoryList[0].Name, "Key to hell");
            Assert.AreEqual(inventoryList[0].Description, "Opens doors");
            Assert.AreEqual(inventoryList[0].Type, ItemType.Key);
        }

        [TestMethod]
        public void Add_And_HasItem_Works()
        {
            var inventory = new Inventory();
            var key = new Item("Key", ItemType.Key, "Opens doors");

            inventory.Add(key);

            Assert.IsTrue(inventory.HasItem(ItemType.Key));
        }

        [TestMethod]
        public void Remove_Item_Works()
        {
            var inventory = new Inventory();
            var sword = new Item("Sword", ItemType.Sword, "Sharp");

            inventory.Add(sword);
            inventory.Remove(sword);

            Assert.IsFalse(inventory.HasItem(ItemType.Sword));
        }

        [TestMethod]
        public void HasItem_False_When_Empty()
        {
            var inventory = new Inventory();
            Assert.IsFalse(inventory.HasItem(ItemType.Key));
            Assert.IsFalse(inventory.HasItem(ItemType.Sword));
            Assert.IsFalse(inventory.HasItem(ItemType.Shield));
        }

        [TestMethod]
        public void HasItem_Distinguishes_Types()
        {
            var inventory = new Inventory();
            var key = new Item("Key", ItemType.Key);
            var sword = new Item("Sword", ItemType.Sword);
            inventory.Add(key);
            inventory.Add(sword);

            Assert.IsTrue(inventory.HasItem(ItemType.Key));
            Assert.IsTrue(inventory.HasItem(ItemType.Sword));
            Assert.IsFalse(inventory.HasItem(ItemType.Shield)); // not added
        }

        [TestMethod]
        public void Remove_NonExisting_Is_NoOp()
        {
            var inventory = new Inventory();
            var key = new Item("Key", ItemType.Key);

            // Attempt to remove item not in inventory
            inventory.Remove(key);

            // Inventory should remain empty, no crash
            Assert.AreEqual(0, inventory.GetAll().Count);
        }

        [TestMethod]
        public void GetAll_Preserves_InsertionOrder()
        {
            var inventory = new Inventory();
            var key = new Item("Key", ItemType.Key);
            var sword = new Item("Sword", ItemType.Sword);
            var shield = new Item("Shield", ItemType.Shield);

            inventory.Add(key);
            inventory.Add(sword);
            inventory.Add(shield);

            var allItems = inventory.GetAll();
            Assert.AreEqual(key, allItems[0]);
            Assert.AreEqual(sword, allItems[1]);
            Assert.AreEqual(shield, allItems[2]);
        }
    }
}