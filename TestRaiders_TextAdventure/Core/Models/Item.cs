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
        private static int _maxId = 0; // internal counter to create unique IDs
        public string Id { get;}
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; private set; }

        // creates a new item with an automatic ID
        public Item(string name, ItemType type, string description)
        {
            _maxId++;
            Id = $"item_{_maxId}";
            Name = name;
            Type = type;

            // if the description is empty, create a default description
            Description = string.IsNullOrWhiteSpace(description)
                ? GenerateDefaultDescription(name)
                : description;
        }

        // additional constructor: creates a new item with only a name.
        // calls the main constructor with an automatically generated description.
        public Item(string name,ItemType type) : this(name,type, "")
        {
            
        }

        // Genereert een standaardbeschrijving wanneer er geen description is opgegeven.
        private static string GenerateDefaultDescription(string name)
            => $"description: {name}.";


        public override string ToString() => $"{Name} ({Id}) - {Description} [{Type}]";
    }
}
