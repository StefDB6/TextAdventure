using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRaiders_TextAdventure.Core.Interfaces
{
    // Publieke interface voor een spel-item.
    public interface IItem
    {
        string Id { get; }
        string Name { get; set; }
        string Description { get; set; }

        string ToString();
    }
}
