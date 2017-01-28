using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Humanizer;
using SimpleJsonApi.Attributes;
using SimpleJsonApi.Configuration.Internal;

namespace SimpleJsonApi.Configuration
{
    /// <summary>
    /// Convenient class to build the resource configuration for passing to the <see cref="JsonApiConfiguration"/>.
    /// </summary>
    public sealed class ResourceConfigurationsBuilder
    {
        private readonly Dictionary<Type, IResourceConfigurationBuilder> _configurationBuilders = new Dictionary<Type, IResourceConfigurationBuilder>();

        /// <summary>
        /// Registers a resource type and returns a builder for further configuring the resource.
        /// </summary>
        /// <typeparam name="TResource">The resource type.</typeparam>
        /// <param name="resourceTypeName">The JSON API resource type name.</param>
        /// <returns>A <see cref="ResourceConfigurationBuilder{TResource}"/> for further configuration.</returns>
        public ResourceConfigurationBuilder<TResource> Resource<TResource>(string resourceTypeName)
        {
            if (string.IsNullOrEmpty(resourceTypeName)) throw new ArgumentNullException(nameof(resourceTypeName));
            if (_configurationBuilders.ContainsKey(typeof(TResource))) throw new ArgumentException($"Resource type {typeof(TResource).Name} already configured");
            var builder = new ResourceConfigurationBuilder<TResource>(resourceTypeName);
            _configurationBuilders.Add(typeof(TResource), builder);
            return builder;
        }

        /// <summary>
        /// Registers a resource type and returns a builder for further configuring the resource.
        /// This overload will guess the JSON API resource type name by using the pluralized form of the <see cref="TResource"/> type name or
        /// the name passed to the <see cref="JsonApiResourceNameAttribute"/> if defined on the resource type.
        /// </summary>
        /// <typeparam name="TResource">The resource type.</typeparam>
        /// <returns></returns>
        public ResourceConfigurationBuilder<TResource> Resource<TResource>()
        {
            var nameAttribute = typeof(TResource).GetCustomAttribute<JsonApiResourceNameAttribute>();
            var name = (nameAttribute?.Name ?? typeof(TResource).Name)
                .ToLower()
                .Pluralize(inputIsKnownToBeSingular: false);
            return Resource<TResource>(name);
        }

        /// <summary>
        /// Build a <see cref="IResourceConfigurations"/> object that can be passed to the <see cref="JsonApiConfiguration"/>.
        /// </summary>
        /// <returns>A <see cref="IResourceConfigurations"/> object containing the resource configurations.</returns>
        public IResourceConfigurations Build()
        {
            return new ResourceConfigurations(_configurationBuilders.ToDictionary(x => x.Key, x => x.Value.Build()));
        }
    }
}
