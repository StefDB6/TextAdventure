using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRaiders_TextAdventure.Core.Models;

namespace Tests
{
    [TestClass]
    public class MonsterTests
    {
        [TestMethod]
        public void Constructor_Sets_Name_And_IsAlive_True()
        {
            Monster monster = new Monster("Dragon");
            Assert.AreEqual("Dragon", monster.Name);
            Assert.IsTrue(monster.IsAlive);
        }

        [TestMethod]
        public void Die_Sets_IsAlive_To_False()
        {
            Monster monster = new Monster("Dragon");
            monster.Die();
            Assert.IsFalse(monster.IsAlive);
        }

        [TestMethod]
        public void Attack_DoesNotChange_IsAlive()
        {
            // Arrange
            var monster = new Monster("Goblin");

            // Act
            monster.Attack();

            // Assert
            Assert.IsTrue(monster.IsAlive, "Monster should still be alive after attacking.");
        }

        [TestMethod]
        public void Multiple_Die_Calls_DoNotThrow()
        {
            // Arrange
            var monster = new Monster("Orc");

            // Act
            monster.Die();
            monster.Die();

            // Assert
            Assert.IsFalse(monster.IsAlive, "Monster should remain dead even if Die() is called multiple times.");
        }

        [TestMethod]
        public void Name_IsReadOnly()
        {
            // Arrange
            var monster = new Monster("Troll");

            // Act & Assert
            // We can't set monster.Name, should be read-only
            Assert.AreEqual("Troll", monster.Name);
        }

        [TestMethod]
        public void IsAlive_IsTrue_Initially()
        {
            // Arrange
            var monster = new Monster("Witch");

            // Assert
            Assert.IsTrue(monster.IsAlive, "Monster should start alive.");
        }
    }
}
