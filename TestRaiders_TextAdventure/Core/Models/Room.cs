using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class Room : IRoom
    {
        private readonly Dictionary<Direction, IRoom> _exits = new();
        private readonly List<IItem> _items = new();

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeadly { get; set; }
        public bool RequiresKey { get; set; }
        public bool HasMonster { get; set; }
        public bool MonsterAlive { get; set; }
        public bool WinningRoom { get; }

        // Expose exits for testing (read-only)
        internal IReadOnlyDictionary<Direction, IRoom> Exits => _exits;

        public Room(string name)
        : this(name, GenerateDefaultDescription(name))
        { }

        public Room(string name, string description, bool isDeadly = false, bool requiresKey = false, bool hasMonster = false, bool winningRoom = false)
        {
            Name = name;
            Description = description;
            IsDeadly = isDeadly;
            RequiresKey = requiresKey;
            HasMonster = hasMonster;
            MonsterAlive = hasMonster;
            WinningRoom = winningRoom;
        }

        public static string GenerateDefaultDescription(string name)
            => $"description: {name}.";

        public void AddExit(Direction direction, IRoom room)
        {
            _exits[direction] = room;
        }

        public IRoom? GetExit(Direction direction)
        {
            _exits.TryGetValue(direction, out var room);
            return room;
        }

        public void AddItem(IItem item)
        {
            _items.Add(item);
        }

        public IItem? TakeItem(string id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item != null)
                _items.Remove(item);
            return item;
        }

        public IList<IItem> GetItems()
        {
            return _items;
        }
    }
}