using System;
using System.Collections.Generic;

namespace SimpleJsonApi.Configuration
{
    public sealed class ResourceConfiguration
    {
        private readonly Dictionary<Type, IResourceMapping> _mappings;

        internal ResourceConfiguration(Dictionary<Type, IResourceMapping> mappings)
        {
            if (mappings == null) throw new ArgumentNullException(nameof(mappings));
            _mappings = mappings;
        }

        internal IResourceMapping GetMappingForType(Type resourceType)
        {
            IResourceMapping result;
            return _mappings.TryGetValue(resourceType, out result) ? result : null;
        }

        internal bool IsMapped(Type resourceType)
        {
            return _mappings.ContainsKey(resourceType);
        }

        internal string GetResourceTypeName(Type resourceType)
        {
            return GetMappingForType(resourceType)?.ResourceTypeName;
        }
    }
}
