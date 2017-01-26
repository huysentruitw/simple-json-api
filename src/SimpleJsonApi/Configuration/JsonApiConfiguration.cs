using System;
using System.Reflection;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SimpleJsonApi.Attributes;

namespace SimpleJsonApi.Configuration
{
    public sealed class JsonApiConfiguration
    {
        public JsonApiConfiguration()
        {
            SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

            BuildResourceTypeName = resourceType =>
            {
                var nameAttribute = resourceType.GetCustomAttribute<JsonApiResourceNameAttribute>();
                var singularName = (nameAttribute?.Name ?? resourceType.Name).ToLower();
                return PluralizeResourceTypeNames ? singularName.Pluralize() : singularName;
            };
        }

        public Func<Type, string> BuildResourceTypeName { get; set; }

        public ResourceConfiguration ResourceConfiguration { get; set; }

        public bool PluralizeResourceTypeNames { get; set; } = true;

        public JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings();

        internal void Validate()
        {
            if (BuildResourceTypeName == null) throw new ArgumentNullException(nameof(BuildResourceTypeName));
            if (ResourceConfiguration == null) throw new ArgumentNullException(nameof(ResourceConfiguration));
        }
    }
}
