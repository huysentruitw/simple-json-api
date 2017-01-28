using System;

namespace SimpleJsonApi.Configuration.Internal
{
    internal sealed class ResourceConfiguration<TResource> : IResourceConfiguration
    {
        private readonly ResourceMapping<TResource> _mapping;

        public ResourceConfiguration(string typeName, ResourceMapping<TResource> mapping)
        {
            TypeName = typeName;
            _mapping = mapping;
        }

        public Type ResourceType => typeof(TResource);

        public string TypeName { get; }

        public IResourceMapping Mapping => _mapping;
    }
}
