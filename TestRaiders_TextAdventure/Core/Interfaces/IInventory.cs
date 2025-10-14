using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRaiders_TextAdventure.Core.Interfaces
{
    internal interface IInventory
    {
        void Add(IItem item);
        void Remove(IItem item);
        IItem HasItem(ItemType item);
        List<IItem> GetAll();
    }
}
