namespace TestRaiders_TextAdventure.Core.Models;

public class Monster
{
    public string Name { get; }
    
    public bool IsAlive { get; private set; } = true;

    public Monster(string name)
    {
        Name = name;
    }

    public void Attack()
    {
        // Simpele attack logic: Voor nu, print message. Later integreren met fight.
        Console.WriteLine($"{Name} attacks!");
    }

    public void Die()
    {
        IsAlive = false;
        Console.WriteLine($"{Name} has been defeated!");
    }
}