using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRaiders_TextAdventure
{
    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Item(string id, string name, string description)
        {
            //Id mag niet leeg of null zijn.
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Item Id mag niet leeg zijn.", nameof(id));

            Id = id;
            Name = name;
            Description = description;
        }

        public override string ToString() => $"{Name} ({Id}) - {Description}";
    }
}
