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
            Item item = new("Key");
            Item item2 = new("Sword");
            inventory.Add(item);
            inventory.Add(item2);
            var test = inventory.GetAll();
            Assert.AreEqual(2, test.Count);
        }

        public void AddItem_Adds_Correct_Item()
        {
            Inventory inventory = new();
            Item item = new("Key");
            Item item2 = new("Sword");
            inventory.Add(item);
            inventory.Add(item2);
            var test = inventory.GetAll();
            Assert.AreEqual(test[0].Name, "Key");
            Assert.AreEqual(test[1].Name, "Sword");
        }
    }
}