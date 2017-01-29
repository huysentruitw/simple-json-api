using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimpleJsonApi.Configuration.Internal;
using SimpleJsonApi.Extensions;

namespace SimpleJsonApi.Configuration
{
    /// <summary>
    /// Provides fluent configuration methods for configuring resource properties and relations.
    /// </summary>
    /// <typeparam name="TResource">The resource type.</typeparam>
    public sealed class ResourceConfigurationBuilder<TResource> : IResourceConfigurationBuilder
    {
        private readonly string _typeName;
        private readonly Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, RelationInfo> _relations = new Dictionary<string, RelationInfo>(StringComparer.OrdinalIgnoreCase);
        private string _idPropertyName;

        internal ResourceConfigurationBuilder(string typeName)
        {
            _typeName = typeName;

            MapAllProperties();
        }

        #region Properties

        /// <summary>
        /// Ignores the given property. This property will not be mapped from/into incoming or outgoing JSON API documents.
        /// </summary>
        /// <param name="expression">Expression that selects the relation property.</param>
        /// <returns>The <see cref="ResourceConfigurationBuilder{TResource}"/> instance for method chaining.</returns>
        public ResourceConfigurationBuilder<TResource> IgnoreProperty(Expression<Func<TResource, object>> expression)
        {
            var propertyInfo = expression.GetPropertyInfoOrThrow();
            UnmapProperty(propertyInfo);
            return this;
        }

        /// <summary>
        /// Marks the given property as the Id property. Only use this if the Id property is not called 'Id'.
        /// </summary>
        /// <param name="expression">Expression that selects the relation property.</param>
        /// <returns>The <see cref="ResourceConfigurationBuilder{TResource}"/> instance for method chaining.</returns>
        public ResourceConfigurationBuilder<TResource> WithIdProperty(Expression<Func<TResource, object>> expression)
        {
            var propertyInfo = expression.GetPropertyInfoOrThrow();
            if (propertyInfo.IsValidIdType()) throw new ArgumentException($"The id property must be of type Guid", nameof(expression));
            _idPropertyName = propertyInfo.Name;
            return this;
        }

        private void MapAllProperties()
        {
            typeof(TResource)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToList()
                .ForEach(MapProperty);
        }

        private void MapProperty(PropertyInfo property)
        {
            ValidateProperty(property);

            var name = property.GetJsonPropertyName();

            if (_properties.ContainsKey(name))
                throw new ArgumentException($"Property with JSON name {name} already mapped", nameof(property));

            // Auto-set Id property
            if (!HasIdProperty && property.IsCandidateForId())
                _idPropertyName = name;

            _properties.Add(name, property);
        }

        private void UnmapProperty(PropertyInfo property)
        {
            var name = property.GetJsonPropertyName();
            _properties.Remove(name);
        }

        private void ValidateProperty(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (!property.CanRead || !property.CanWrite)
                throw new ArgumentException($"Property '{property.Name}' must have a getter and a setter");
        }

        private bool HasIdProperty => !string.IsNullOrEmpty(_idPropertyName) && _properties.ContainsKey(_idPropertyName);

        #endregion

        #region Relations

        /// <summary>
        /// Marks a property as a single relation.
        /// </summary>
        /// <typeparam name="TRelatedResource">The resource type of the related resource.</typeparam>
        /// <param name="expression">Expression that selects the relation property.</param>
        /// <returns>The <see cref="ResourceConfigurationBuilder{TResource}"/> instance for method chaining.</returns>
        public ResourceConfigurationBuilder<TResource> BelongsTo<TRelatedResource>(Expression<Func<TResource, object>> expression)
        {
            var propertyInfo = expression.GetPropertyInfoOrThrow();
            MapRelation(propertyInfo, typeof(TRelatedResource), RelationKind.BelongsTo);
            return this;
        }

        /// <summary>
        /// Marks a property as a to-many relation.
        /// </summary>
        /// <typeparam name="TRelatedResource">The resource type of the related resource.</typeparam>
        /// <param name="expression">Expression that selects the relation property.</param>
        /// <returns>The <see cref="ResourceConfigurationBuilder{TResource}"/> instance for method chaining.</returns>
        public ResourceConfigurationBuilder<TResource> HasMany<TRelatedResource>(Expression<Func<TResource, object>> expression)
        {
            var propertyInfo = expression.GetPropertyInfoOrThrow();
            MapRelation(propertyInfo, typeof(TRelatedResource), RelationKind.HasMany);
            return this;
        }

        private void MapRelation(PropertyInfo property, Type relatedResourceType, RelationKind kind)
        {
            var name = property.GetJsonPropertyName();

            if (!_properties.ContainsKey(name))
                throw new ArgumentException($"Property with JSON name {name} not found. Did you call {nameof(IgnoreProperty)} on the property?");

            if (_relations.ContainsKey(name))
                throw new ArgumentException($"Relation property with JSON name {name} already mapped", nameof(property));

            _relations.Add(name, new RelationInfo(property, relatedResourceType, kind));
        }

        #endregion

        private void Validate()
        {
            if (!HasIdProperty)
                throw new Exception($"No id property defined for resource {_typeName}");

            if (_relations.ContainsKey(_idPropertyName))
                throw new Exception($"Id property {_idPropertyName} cannot be marked as a relationship");
        }

        IResourceConfiguration IResourceConfigurationBuilder.Build()
        {
            Validate();

            var attributes = _properties
                .Where(x => !_relations.ContainsKey(x.Key) && !_idPropertyName.Equals(x.Key, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

            var idProperty = new IdPropertyInfo(_idPropertyName, _properties[_idPropertyName]);

            var mapping = new ResourceMapping<TResource>(attributes, _relations, idProperty);

            return new ResourceConfiguration<TResource>(_typeName, mapping);
        }
    }
}
