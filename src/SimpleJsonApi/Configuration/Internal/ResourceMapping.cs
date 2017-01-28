using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleJsonApi.Configuration.Internal
{
    internal sealed class ResourceMapping<TResource> : IResourceMapping
    {
        private readonly Dictionary<string, PropertyInfo> _attributeProperties;
        private readonly Dictionary<string, RelationInfo> _relationProperties;
        private readonly IdPropertyInfo _idProperty;

        public ResourceMapping(
            Dictionary<string, PropertyInfo> attributeProperties,
            Dictionary<string, RelationInfo> relationProperties,
            IdPropertyInfo idProperty)
        {
            _attributeProperties = attributeProperties;
            _relationProperties = relationProperties;
            _idProperty = idProperty;
        }

        public void SetId(object instance, Guid id) => _idProperty.Property.SetValue(instance, id);

        public Type GetAttributeType(string name) => GetAttributeInfo(name)?.PropertyType;

        public void SetAttributeValue(object instance, string name, object value) => GetAttributeInfo(name)?.SetValue(instance, value);

        public object GetAttributeValue(object instance, string name) => GetAttributeInfo(name).GetValue(instance);

        public Type GetResourceTypeOfRelation(string name) => GetRelationInfo(name)?.ResourceType;

        public bool IsHasManyRelation(string name) => GetRelationInfo(name)?.Kind == RelationKind.HasMany;

        public void SetRelationValue(object instance, string name, Guid relationId)
            => GetRelationInfo(name)?.PropertyInfo.SetValue(instance, relationId);

        public void SetRelationValues(object instance, string name, IEnumerable<Guid> relationIds)
            => GetRelationInfo(name)?.PropertyInfo.SetValue(instance, relationIds);

        private PropertyInfo GetAttributeInfo(string name)
        {
            PropertyInfo info;
            return _attributeProperties.TryGetValue(name, out info) ? info : null;
        }

        private RelationInfo GetRelationInfo(string name)
        {
            RelationInfo info;
            return _relationProperties.TryGetValue(name, out info) ? info : null;
        }
    }
}
