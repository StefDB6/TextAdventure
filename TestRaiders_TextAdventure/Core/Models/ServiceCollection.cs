using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRaiders_TextAdventure.Core.Interfaces;


namespace TestRaiders_TextAdventure.Core.Models
{
    public class ServiceCollection : IServiceCollection
    {
        // Internal dictionary that holds service-to-implementation mappings
        private readonly Dictionary<Type, Type> _singletons = new();

        // Registers a singleton mapping between a service and its implementation
        public void AddSingleton<TService, TImplementation>()
            where TImplementation : TService
        {
            _singletons[typeof(TService)] = typeof(TImplementation);
        }

        // Checks whether a given service is registered with the specified implementation.
        public bool IsRegistered<TService, TImplementation>()
            where TImplementation : TService
        {
            return _singletons.TryGetValue(typeof(TService), out var impl)
                   && impl == typeof(TImplementation);
        }
    }
}

