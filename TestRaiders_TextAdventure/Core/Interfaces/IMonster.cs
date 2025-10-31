using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRaiders_TextAdventure.Core.Interfaces
{
    public interface IMonster
    {
        string Name { get; }
        bool IsAlive { get; }
        void Attack();
        void Die();
    }
}
