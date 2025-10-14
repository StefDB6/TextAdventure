using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class Inventory : IInventory
    {
        private List<IItem> inventory = new();

        public void Add(IItem item)
        {
            inventory.Add(item);
        }
        public void Remove(IItem item)
        {
            throw new NotImplementedException();
        }
        public IItem HasItem(ItemType item)
        {
            throw new NotImplementedException();
        }
        public List<IItem> GetAll()
        {
            return inventory;
        }



    }
}
