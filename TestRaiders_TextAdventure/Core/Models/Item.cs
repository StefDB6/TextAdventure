using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRaiders_TextAdventure.Core.Interfaces;

namespace TestRaiders_TextAdventure.Core.Models
{
    public class Item : IItem
    {
        private static int _maxId = 0; // interne teller om unieke IDs te maken
        public string Id { get;}
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
