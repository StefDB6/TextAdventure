using TestRaiders_TextAdventure;
using TestRaiders_TextAdventure.Core.Models;
namespace Tests
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        //werkt de constructor wanneer je een Item object aanmaakt
        public void Constructor_Sets_Properties_And_Generates_Id()
        {
            var item = new Item("Sleutel", "Opent deur");

            Assert.IsNotNull(item.Id, "Id mag niet null zijn.");
            StringAssert.StartsWith(item.Id, "item_");
            Assert.AreEqual("Sleutel", item.Name);
            Assert.AreEqual("Opent deur", item.Description);
        }

        [TestMethod]
        // werkt de tweede constructor (met chaining) zonder description
        public void Constructor_Chaining_Creates_AutoDescription()
        {
            var item = new Item("Sleutel"); // gebruikt constructor chaining

            Assert.AreEqual("Sleutel", item.Name);
            Assert.AreEqual("beschrijving: Sleutel.", item.Description);
        }

        [TestMethod]
        // werkt de tweede constructor (met chaining) voor een random ID
        public void Constructor_Chaining_Creates_Id()
        {
            var item = new Item("Sleutel"); // gebruikt constructor chaining

            Assert.IsNotNull(item.Id);
            StringAssert.StartsWith(item.Id, "item_");
        }

        [TestMethod]
        // lege beschrijving krijgt ook automatisch een tekst
        public void Empty_Description_Generates_Default_Description()
        {
            var item = new Item("Zwaard", "");

            Assert.AreEqual("beschrijving: Zwaard.", item.Description);
        }

        // Test dat twee verschillende items unieke IDs krijgen.
        [TestMethod]
        public void Each_Item_Has_Unique_Id()
        {
            var i1 = new Item("Sleutel", "Opent deur");
            var i2 = new Item("Zwaard", "Tegen monsters");

            Assert.AreNotEqual(i1.Id, i2.Id, "IDs moeten uniek zijn.");
        }

        [TestMethod]

        //props zijn niet read-only dus we moeten testen of deze nog wijzigbaar zijn
        public void Properties_Are_Mutable() 
        {
            var item = new Item("Zwaard", "Tegen monsters");

            item.Name = "Stalen zwaard";
            item.Description = "Scherp";

            Assert.AreEqual("Stalen zwaard", item.Name);
            Assert.AreEqual("Scherp", item.Description);
        }

        [TestMethod]
        // Test dat de ToString()-methode van Item een leesbare tekst teruggeeft 
        // die minstens de naam en het ID van het item bevat.
        // Zo weten we dat de methode nuttige informatie toont voor de speler.
        public void ToString_Contains_Name_And_Id()
        {
            var item = new Item("Sleutel", "Opent deur");
            var s = item.ToString();

            StringAssert.Contains(s, "Sleutel");
            StringAssert.Contains(s, item.Id);
        }

    }
}
