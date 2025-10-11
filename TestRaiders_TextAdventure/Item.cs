using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRaiders_TextAdventure
{
    public class Item
    {
        private static int _maxId = 0; // interne teller om unieke IDs te maken
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Maakt een nieuw item aan met een automatisch ID.
        public Item(string name, string description)
        {
            _maxId++;
            Id = $"item_{_maxId}";
            Name = name;
            Description = description;
        }

        public override string ToString() => $"{Name} ({Id}) - {Description}";
    }
}
