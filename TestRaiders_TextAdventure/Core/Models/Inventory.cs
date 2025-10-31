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
        private readonly List<IItem> _items = new();

        public void Add(IItem item)
        {
            _items.Add(item);
        }
        public void Remove(IItem item)
        {
            _items.Remove(item);
        }
        public bool HasItem(ItemType type)
        {
            return _items.Any(i => i.Type == type);
        }
        public List<IItem> GetAll()
        {
            return _items;
        }



    }
}
