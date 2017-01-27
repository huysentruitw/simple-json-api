using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using SimpleJsonApi.Extensions;

namespace SimpleJsonApi.Configuration
{
    public sealed class ResourceMappingBuilder<TResource> : IResourceMappingBuilder
    {
        private readonly string _resourceTypeName;
        private readonly Type _resourceType = typeof(TResource);
        private readonly Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, RelationInfo> _relations = new Dictionary<string, RelationInfo>(StringComparer.OrdinalIgnoreCase);
        private string _idPropertyName;

        internal ResourceMappingBuilder(string resourceTypeName)
        {
            if (string.IsNullOrEmpty(resourceTypeName)) throw new ArgumentNullException(nameof(resourceTypeName));
            _resourceTypeName = resourceTypeName;
        }

        public ResourceMappingBuilder<TResource> WithAllProperties()
        {
            typeof(TResource)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToList()
                .ForEach(AddProperty);
            return this;
        }

        public ResourceMappingBuilder<TResource> WithProperty(Expression<Func<TResource, object>> expression)
        {
            var propertyInfo = expression.GetPropertyInfo();
            if (propertyInfo == null) throw new ArgumentException("Failed to get property info from expression", nameof(expression));
            AddProperty(propertyInfo);
            return this;
        }

        public ResourceMappingBuilder<TResource> WithoutProperty(Expression<Func<TResource, object>> expression)
        {
            var propertyInfo = expression.GetPropertyInfo();
            if (propertyInfo == null) throw new ArgumentException("Failed to get property info from expression", nameof(expression));
            RemoveProperty(propertyInfo.Name);
            return this;
        }

        public ResourceMappingBuilder<TResource> WithIdProperty(Expression<Func<TResource, object>> expression)
        {
            var propertyInfo = expression.GetPropertyInfo();
            if (propertyInfo == null) throw new ArgumentException("Failed to get property info from expression", nameof(expression));
            if (propertyInfo.PropertyType != typeof(Guid)) throw new ArgumentException("Id property must be of type Guid", nameof(expression));
            AddProperty(propertyInfo);
            _idPropertyName = propertyInfo.Name;
            return this;
        }

        public ResourceMappingBuilder<TResource> WithOne<TRelatedResourceType>(Expression<Func<TResource, object>> expression)
        {
            var propertyInfo = expression.GetPropertyInfo();
            if (propertyInfo == null) throw new ArgumentException("Failed to get property info from expression", nameof(expression));
            AddRelation(propertyInfo, typeof(TRelatedResourceType), false);
            return this;
        }

        public ResourceMappingBuilder<TResource> WithMany<TRelatedResourceType>(Expression<Func<TResource, object>> expression)
        {
            var propertyInfo = expression.GetPropertyInfo();
            if (propertyInfo == null) throw new ArgumentException("Failed to get property info from expression", nameof(expression));
            AddRelation(propertyInfo, typeof(TRelatedResourceType), true);
            return this;
        }

        IResourceMapping IResourceMappingBuilder.Build()
        {
            // Remove all relation related properties from the _properties map
            _relations.Keys.ToList().ForEach(RemoveProperty);

            Validate();

            return new ResourceMapping<TResource>(_resourceTypeName, _properties, _relations, _idPropertyName);
        }

        private bool HasIdProperty()
        {
            return !string.IsNullOrEmpty(_idPropertyName) && _properties.ContainsKey(_idPropertyName);
        }

        private void Validate()
        {
            if (!HasIdProperty()) throw new Exception($"No Id property of type Guid found for resource {_resourceType.Name}");
            if (_relations.ContainsKey(_idPropertyName)) throw new Exception($"Id property {_idPropertyName} cannot be marked as a relationship");
        }

        private void AddProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead || !propertyInfo.CanWrite) throw new ArgumentException($"Property {propertyInfo.Name} must have a getter and a setter");

            var propertyName = GetPropertyName(propertyInfo);

            if (IsCandidateForId(propertyInfo) && !HasIdProperty())
                _idPropertyName = propertyName;

            if (!_properties.ContainsKey(propertyName))
                _properties.Add(propertyName, propertyInfo);
        }

        private void RemoveProperty(string name)
        {
            _properties.Remove(name);
            if (name.Equals(_idPropertyName)) _idPropertyName = null;
        }

        private void AddRelation(PropertyInfo propertyInfo, Type relatedResourceType, bool hasMany)
        {
            if (!propertyInfo.CanRead || !propertyInfo.CanWrite) throw new ArgumentException($"Relation property {propertyInfo.Name} must have a getter and a setter");
            var propertyName = GetPropertyName(propertyInfo);
            if (_relations.ContainsKey(propertyName)) throw new InvalidOperationException($"Relation {propertyName} already defined");
            _relations.Add(propertyName, new RelationInfo(propertyInfo, relatedResourceType, hasMany));
        }

        private static string GetPropertyName(PropertyInfo propertyInfo)
        {
            var jsonProperty = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            return jsonProperty?.PropertyName ?? propertyInfo.Name;
        }

        private static bool IsCandidateForId(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name.Equals("id", StringComparison.OrdinalIgnoreCase) && propertyInfo.PropertyType == typeof(Guid);
        }
    }
}
