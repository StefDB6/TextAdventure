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
    }
}