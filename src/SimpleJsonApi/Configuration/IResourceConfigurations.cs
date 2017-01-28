using System;

namespace SimpleJsonApi.Configuration
{
    public interface IResourceConfigurations
    {
        /// <summary>
        /// Lookup the <see cref="IResourceConfiguration"/> for the given resource type.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <returns>The <see cref="IResourceConfiguration"/> or null in case the type is unknown.</returns>
        IResourceConfiguration this[Type resourceType] { get; }

        /// <summary>
        /// Checks if the collection contains a <see cref="IResourceConfiguration"/> for the given resource type.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <returns>True if the <see cref="IResourceConfiguration"/> exists for the given resource type, false when not.</returns>
        bool Contains(Type resourceType);
    }
}
