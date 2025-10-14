using TestRaiders_TextAdventure;
using TestRaiders_TextAdventure.Core.Models;
namespace Tests
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        //does the constructor work when you create an Item object
        public void Constructor_Sets_Properties_And_Generates_Id()
        {
            var item = new Item("Key", "Opens door");

            Assert.IsNotNull(item.Id, "Id can not be null.");
            StringAssert.StartsWith(item.Id, "item_");
            Assert.AreEqual("Key", item.Name);
            Assert.AreEqual("Opens door", item.Description);
        }

        [TestMethod]
        // does the second constructor (with chaining) work without a description
        public void Constructor_Chaining_Creates_AutoDescription()
        {
            var item = new Item("Key"); // uses constructor chaining

            Assert.AreEqual("Key", item.Name);
            Assert.AreEqual("description: Key.", item.Description);
        }

        [TestMethod]
        // does the second constructor (with chaining) work for a random ID
        public void Constructor_Chaining_Creates_Id()
        {
            var item = new Item("Key"); // uses constructor chaining

            Assert.IsNotNull(item.Id);
            StringAssert.StartsWith(item.Id, "item_");
        }

        [TestMethod]
        // an empty description also automatically gets a text
        public void Empty_Description_Generates_Default_Description()
        {
            var item = new Item("Sword", "");

            Assert.AreEqual("description: Sword.", item.Description);
        }

        // test that two different items get unique IDs
        [TestMethod]
        public void Each_Item_Has_Unique_Id()
        {
            var i1 = new Item("Key", "Opens door");
            var i2 = new Item("Sword", "Against Monsters");

            Assert.AreNotEqual(i1.Id, i2.Id, "IDs have to be unique.");
        }

        [TestMethod]

        // properties are not read-only, so we need to test whether they can still be modified
        public void Properties_Are_Mutable() 
        {
            var item = new Item("Sword", "Against Monsters");

            item.Name = "Steel sword";
            item.Description = "Sharp";

            Assert.AreEqual("Steel sword", item.Name);
            Assert.AreEqual("Sharp", item.Description);
        }

        [TestMethod]
        // test that the ToString() method of Item returns a readable text
        // that includes at least the name and ID of the item.
        // this way, we know that the method shows useful information for the player.
        public void ToString_Contains_Name_And_Id()
        {
            var item = new Item("Sleutel", "Opent deur");
            var s = item.ToString();

            StringAssert.Contains(s, "Sleutel");
            StringAssert.Contains(s, item.Id);
        }

    }
}
