using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TestRaiders_TextAdventure.Core.Interfaces
{
    public interface IRoomsManager
    {
        void Go(Direction direction);
        void Look();
        void Take(string itemId);
        void Fight();

        bool HasWon();
        bool IsGameOver { get; }
    }
}
