using System;
using System.Reflection;
using Newtonsoft.Json;

namespace SimpleJsonApi.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static string GetJsonPropertyName(this PropertyInfo property)
            => property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? property.Name;

        public static bool IsCandidateForId(this PropertyInfo property)
            => property.IsValidIdType() && property.GetJsonPropertyName().Equals("id", StringComparison.OrdinalIgnoreCase);

        public static bool IsValidIdType(this PropertyInfo property)
            => property.PropertyType == typeof(Guid);
    }
}
