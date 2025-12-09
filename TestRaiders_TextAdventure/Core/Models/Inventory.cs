using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class Inventory : IInventory
    {
        private readonly List<IItem> _items = new();

        public void Add(IItem item)
        {
            _items.Add(item);
            return;
        }
        public void Remove(IItem item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
                return;
            }
            return;
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
