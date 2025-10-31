using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void Die()
        {
            IsAlive = false;
            Console.WriteLine($"{Name} has been defeated!");
        }
    }
}
