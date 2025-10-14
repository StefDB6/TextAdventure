using TestRaiders_TextAdventure.Core.Models;

namespace Tests;

[TestClass]
public class MonsterTests
{
    [TestMethod]
    public void Constructor_SetsNameAndIsAliveTrue()
    {
        Monster monster = new Monster("Dragon");
        Assert.AreEqual("Dragon", monster.Name);
        Assert.IsTrue(monster.IsAlive);
    }

    [TestMethod]
    public void Die_SetsIsAliveToFalse()
    {
        Monster monster = new Monster("Dragon");
        monster.Die();
        Assert.IsFalse(monster.IsAlive);
    }

    // Attack() is tekst output, een Console.WriteLine test men niet normaal gezien, kan wel ge-moqt worden

    [TestMethod]
    public void Attack_DoesNotThrowException()
    {
        var monster = new Monster("Dragon");
        monster.Attack();  // Verifieert dat het runt
    }
}