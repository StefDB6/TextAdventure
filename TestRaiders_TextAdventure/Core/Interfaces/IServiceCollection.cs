using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRaiders_TextAdventure.Core.Interfaces
{
    public interface IServiceCollection
    {
        void AddSingleton<TService, TImplementation>()
            where TImplementation : TService;

        bool IsRegistered<TService, TImplementation>()
            where TImplementation : TService;
    }
}
