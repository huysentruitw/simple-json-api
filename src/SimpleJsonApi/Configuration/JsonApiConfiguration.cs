using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SimpleJsonApi.Configuration
{
    /// <summary>
    /// The configuration class for configuring <see cref="SimpleJsonApi"/>.
    /// </summary>
    public sealed class JsonApiConfiguration
    {
        /// <summary>
        /// Creates a <see cref="JsonApiConfiguration"/> instance.
        /// </summary>
        public JsonApiConfiguration()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        }

        /// <summary>
        /// The MediaType to react on. By default this is set to 'application/vnd.api+json' as per the JSON API 1.0 specification.
        /// </summary>
        public string MediaType { get; set; } = Constants.JsonApiMediaType;

        /// <summary>
        /// The resource configurations collection.<br />
        /// Use the <see cref="ResourceConfigurationsBuilder"/> to build this configuration or roll your own implementation.
        /// </summary>
        public IResourceConfigurations ResourceConfigurations { get; set; }

        /// <summary>
        /// The serializer settings. By default, the serializer is configured for camelCasing and to use ISO dates.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings();

        internal void Validate()
        {
            if (string.IsNullOrEmpty(MediaType)) throw new ArgumentNullException(nameof(MediaType));
            if (ResourceConfigurations == null) throw new ArgumentNullException(nameof(ResourceConfigurations));
        }
    }
}
