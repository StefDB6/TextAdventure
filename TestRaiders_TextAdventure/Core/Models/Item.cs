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

            // Als de beschrijving leeg is, maak een standaardbeschrijving aan
            Description = string.IsNullOrWhiteSpace(description)
                ? GenerateDefaultDescription(name)
                : description;
        }

        // Extra constructor: maakt een nieuw item met enkel een naam.
        // Roept de hoofdconstructor aan met een automatisch gegenereerde beschrijving.
        public Item(string name) : this(name, "")
        {

        }

        // Genereert een standaardbeschrijving wanneer er geen description is opgegeven.
        private static string GenerateDefaultDescription(string name)
            => $"beschrijving: {name}.";


        public override string ToString() => $"{Name} ({Id}) - {Description}";
    }
}
