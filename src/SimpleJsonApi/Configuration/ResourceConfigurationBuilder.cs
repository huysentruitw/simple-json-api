using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Humanizer;
using SimpleJsonApi.Attributes;

namespace SimpleJsonApi.Configuration
{
    public sealed class ResourceConfigurationBuilder
    {
        private readonly Dictionary<Type, IResourceMappingBuilder> _mappingBuilders = new Dictionary<Type, IResourceMappingBuilder>();

        public ResourceMappingBuilder<TResource> Resource<TResource>(string resourceTypeName)
        {
            if (string.IsNullOrEmpty(resourceTypeName)) throw new ArgumentNullException(nameof(resourceTypeName));
            if (_mappingBuilders.ContainsKey(typeof(TResource))) throw new ArgumentException($"Resource type {typeof(TResource).Name} already configured");
            var mapping = new ResourceMappingBuilder<TResource>(resourceTypeName);
            _mappingBuilders.Add(typeof(TResource), mapping);
            return mapping;
        }

        public ResourceMappingBuilder<TResource> Resource<TResource>()
        {
            var nameAttribute = typeof(TResource).GetCustomAttribute<JsonApiResourceNameAttribute>();
            var singularName = (nameAttribute?.Name ?? typeof(TResource).Name).ToLower();
            return Resource<TResource>(singularName.Pluralize());
        }

        public ResourceConfiguration Build()
        {
            var mappings = _mappingBuilders.ToDictionary(x => x.Key, x => x.Value.Build());
            return new ResourceConfiguration(mappings);
        }
    }
}
