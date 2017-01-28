using System;
using System.Collections.Generic;

namespace SimpleJsonApi.Configuration.Internal
{
    internal sealed class ResourceConfigurations : IResourceConfigurations
    {
        private readonly Dictionary<Type, IResourceConfiguration> _resourceConfigurations;

        public ResourceConfigurations(Dictionary<Type, IResourceConfiguration> resourceConfigurations)
        {
            _resourceConfigurations = resourceConfigurations;
        }

        public IResourceConfiguration this[Type resourceType]
        {
            get
            {
                IResourceConfiguration configuration;
                return _resourceConfigurations.TryGetValue(resourceType, out configuration)
                    ? configuration
                    : null;
            }
        }

        public bool Contains(Type resourceType) => _resourceConfigurations.ContainsKey(resourceType);
    }
}
