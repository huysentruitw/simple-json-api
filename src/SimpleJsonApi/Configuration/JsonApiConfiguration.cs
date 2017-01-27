using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SimpleJsonApi.Configuration
{
    public sealed class JsonApiConfiguration
    {
        public const string JsonApiMediaType = "application/vnd.api+json";

        public JsonApiConfiguration()
        {
            SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        }

        public string MediaType { get; set; } = JsonApiMediaType;

        public ResourceConfiguration ResourceConfiguration { get; set; }

        public JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings();

        internal void Validate()
        {
            if (string.IsNullOrEmpty(MediaType)) throw new ArgumentNullException(nameof(MediaType));
            if (ResourceConfiguration == null) throw new ArgumentNullException(nameof(ResourceConfiguration));
        }
    }
}
