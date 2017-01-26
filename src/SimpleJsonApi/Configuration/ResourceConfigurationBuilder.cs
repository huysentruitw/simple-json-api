using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleJsonApi.Configuration
{
    public sealed class ResourceConfigurationBuilder
    {
        private readonly Dictionary<Type, IResourceMappingBuilder> _mappingBuilders = new Dictionary<Type, IResourceMappingBuilder>();

        public ResourceMappingBuilder<TResource> Resource<TResource>()
        {
            var mapping = new ResourceMappingBuilder<TResource>();
            _mappingBuilders.Add(typeof(TResource), mapping);
            return mapping;
        }

        public ResourceConfiguration Build()
        {
            var mappings = _mappingBuilders.ToDictionary(x => x.Key, x => x.Value.Build());
            return new ResourceConfiguration(mappings);
        }
    }
}
