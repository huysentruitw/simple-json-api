using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SimpleJsonApi.Configuration
{
    public sealed class JsonApiConfiguration
    {
        public JsonApiConfiguration()
        {
            SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        }

        public ResourceConfiguration ResourceConfiguration { get; set; }

        public bool PluralizeResourceTypeNames { get; set; } = true;

        public JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings();

        internal void Validate()
        {
            if (ResourceConfiguration == null) throw new ArgumentNullException(nameof(ResourceConfiguration));
        }
    }
}
