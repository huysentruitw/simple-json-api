using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleJsonApi.Configuration
{
    internal sealed class ResourceMapping<TResource> : IResourceMapping
    {
        private readonly Dictionary<string, PropertyInfo> _properties;
        private readonly Dictionary<string, RelationInfo> _relations;
        private readonly string _idPropertyName;

        public ResourceMapping(string resourceTypeName, Dictionary<string, PropertyInfo> properties, Dictionary<string, RelationInfo> relations, string idPropertyName)
        {
            if (string.IsNullOrEmpty(resourceTypeName)) throw new ArgumentNullException(nameof(resourceTypeName));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (relations == null) throw new ArgumentNullException(nameof(relations));
            if (idPropertyName == null) throw new ArgumentNullException(nameof(idPropertyName));
            ResourceTypeName = resourceTypeName;
            _properties = properties;
            _relations = relations;
            _idPropertyName = idPropertyName;
        }

        public string ResourceTypeName { get; }

        public void SetId(object instance, Guid id)
        {
            SetProperty(instance, _idPropertyName, id);
        }

        public Type GetPropertyType(string propertyName)
        {
            PropertyInfo propertyInfo;
            return _properties.TryGetValue(propertyName, out propertyInfo) ? propertyInfo.PropertyType : null;
        }

        public void SetProperty(object instance, string propertyName, object propertyValue)
        {
            ValidateInstance(instance);
            GetPropertyInfo(propertyName).SetValue(instance, propertyValue);
        }

        public object GetProperty(object instance, string propertyName)
        {
            ValidateInstance(instance);
            return GetPropertyInfo(propertyName).GetValue(instance);
        }

        public Type GetResourceTypeOfRelation(string propertyName)
        {
            RelationInfo relationInfo;
            return _relations.TryGetValue(propertyName, out relationInfo) ? relationInfo.ResourceType : null;
        }

        public bool HasManyRelation(string propertyName)
        {
            RelationInfo relationInfo;
            return _relations.TryGetValue(propertyName, out relationInfo) && relationInfo.HasMany;
        }

        public void SetRelation(object instance, string relationPropertyName, Guid relationId)
        {
            ValidateInstance(instance);
            GetRelationPropertyInfo(relationPropertyName).SetValue(instance, relationId);
        }

        public void SetRelations(object instance, string relationPropertyName, IEnumerable<Guid> relationIds)
        {
            ValidateInstance(instance);
            GetRelationPropertyInfo(relationPropertyName).SetValue(instance, relationIds);
        }

        private void ValidateInstance(object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (instance.GetType() != typeof(TResource))
                throw new ArgumentException(
                    $"Can't set Id on type {instance.GetType().Name} with mapping for {typeof(TResource).Name}",
                    nameof(instance));
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));
            if (!_properties.ContainsKey(propertyName)) throw new ArgumentException($"Property {propertyName} not found/registered on {typeof(TResource).Name}");
            return _properties[propertyName];
        }

        private PropertyInfo GetRelationPropertyInfo(string relationPropertyName)
        {
            if (string.IsNullOrEmpty(relationPropertyName)) throw new ArgumentNullException(nameof(relationPropertyName));
            if (!_relations.ContainsKey(relationPropertyName)) throw new ArgumentException($"Relation property {relationPropertyName} not found/registered on {typeof(TResource).Name}");
            return _relations[relationPropertyName].PropertyInfo;
        }
    }
}
