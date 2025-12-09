using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class Monster : IMonster
    {
        public string Name { get; }
        public bool IsAlive { get; private set; } = true;
        public Monster(string name)
        {
            Name = name;
        }

        public int Attack()
        {
            Console.WriteLine($"{Name} attacks!");
            return 10;
        }

        public string Die()
        {
            IsAlive = false;
            return $"{Name} has been defeated!";
        }
    }
}
