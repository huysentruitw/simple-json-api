using System;
using System.Collections.Generic;

namespace SimpleJsonApi.Configuration
{
    /// <summary>
    /// Holds member information and mapping methods for a single resource.
    /// </summary>
    public interface IResourceMapping
    {
        /// <summary>
        /// Sets the id property of a resource instance using the correct mapping.
        /// </summary>
        /// <param name="instance">The resource instance.</param>
        /// <param name="id">The id.</param>
        void SetId(object instance, Guid id);

        /// <summary>
        /// Gets the id property of a resource instance using the correct mapping.
        /// </summary>
        /// <param name="instance">The resource instance.</param>
        /// <returns>The id.</returns>
        Guid GetId(object instance);

        /// <summary>
        /// Gets a list of mapped attribute names.
        /// </summary>
        /// <returns>The list of mapped attribute names.</returns>
        IEnumerable<string> GetAttributeNames();

        /// <summary>
        /// Gets the type of an attribute property.
        /// </summary>
        /// <param name="name">The JSON API name of the property.</param>
        /// <returns>The property type of the attribute.</returns>
        Type GetAttributeType(string name);

        /// <summary>
        /// Sets the value of an attribute property on the given resource instance.
        /// </summary>
        /// <param name="instance">The resource instance.</param>
        /// <param name="name">The JSON API name of the property.</param>
        /// <param name="value">The value.</param>
        void SetAttributeValue(object instance, string name, object value);

        /// <summary>
        /// Gets the value of an attribute from a resource instance using the correct mapping.
        /// </summary>
        /// <param name="instance">The resource instance.</param>
        /// <param name="name">The JSON API name of the property.</param>
        /// <returns>The value.</returns>
        object GetAttributeValue(object instance, string name);

        /// <summary>
        /// Gets a list of mapped relation names.
        /// </summary>
        /// <returns>The list of mapped relation names.</returns>
        IEnumerable<string> GetRelationNames();

        /// <summary>
        /// Gets the resource type of the relation identified by its JSON API name.
        /// </summary>
        /// <param name="name">The JSON API name of the relation property.</param>
        /// <returns>The resource type of the related resource.</returns>
        Type GetResourceTypeOfRelation(string name);

        /// <summary>
        /// Gets a value that indicates whether the relation is a HasMany or a BelongsTo relation.
        /// </summary>
        /// <param name="name">The JSON API name of the relation property.</param>
        /// <returns>True for HasMany relation, false for a BelongsTo relation.</returns>
        bool IsHasManyRelation(string name);

        /// <summary>
        /// Sets the relation id for a BelongsTo relation.
        /// </summary>
        /// <param name="instance">The resource instance.</param>
        /// <param name="name">The JSON API name of the relation property.</param>
        /// <param name="relationId">The id of the related resource.</param>
        void SetRelationValue(object instance, string name, Guid relationId);

        /// <summary>
        /// Sets the relation ids for a HasMany relation.
        /// </summary>
        /// <param name="instance">The resource instance.</param>
        /// <param name="name">The JSON API name of the relation property.</param>
        /// <param name="relationIds">The ids of the related resource.</param>
        void SetRelationValue(object instance, string name, IEnumerable<Guid> relationIds);

        /// <summary>
        /// Gets the relation id(s) of a relation.
        /// </summary>
        /// <param name="instance">The resource instance.</param>
        /// <param name="name">The JSON API name of the relation property.</param>
        /// <returns>The id(s) of the related resource.</returns>
        object GetRelationValue(object instance, string name);
    }
}
