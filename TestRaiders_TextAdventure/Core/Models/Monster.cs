namespace TestRaiders_TextAdventure.Core.Models;

public class Monster
{
    private string name;
    public string Name 
    { 
        get { return name; } 
    }
    
    private bool isAlive = true;
    public bool IsAlive 
    { 
        get { return isAlive; }
        private set { isAlive = value; }
    }

    public Monster(string name)
    {
        this.name = name;
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