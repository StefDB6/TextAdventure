using TestRaiders_TextAdventure;
namespace Tests
{
    [TestClass]
    public class ItemTests
    {
        [TestMethod]
        //werkt de constructor wanneer je een Item object aanmaakt
        public void Constuctor_Sets_All_Properties()
        {
            var item = new Item("key", "Sleutel", "Opent een deur");

            Assert.AreEqual("key", item.Id);
            Assert.AreEqual("Sleutel", item.Name);
            Assert.AreEqual("Opent een deur", item.Description);
        }

        [TestMethod]

        //props zijn niet read-only dus we moeten testen of deze nog wijzigbaar zijn
        public void Properties_Are_Mutable() 
        {
            var item = new Item("sword", "Zwaard", "Tegen monsters");

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
            var item = new Item("key", "Sleutel", "Opent deur");
            var s = item.ToString();

            StringAssert.Contains(s, "Sleutel");
            StringAssert.Contains(s, "key");
        }

        [TestMethod]
        // we verwachten dat een lege Id wordt geweigerd.
        // Bij fout: ArgumentException.
        public void Id_Should_Not_Be_Null_Or_Empty()
        {
            // we verwachten dat een lege Id wordt geweigerd.
            // Bij fout: ArgumentException.
            Assert.ThrowsException<ArgumentException>(() => new Item("", "X", "Y"));
            Assert.ThrowsException<ArgumentException>(() => new Item(null!, "X", "Y"));
        }
    }
}
