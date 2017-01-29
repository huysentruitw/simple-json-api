using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleJsonApi.Extensions
{
    internal static class ObjectExtensions
    {
        public static object ParseAs(this object @object, Type objectType, JsonSerializer jsonSerializer)
            => JToken.FromObject(@object, jsonSerializer).ToObject(objectType, jsonSerializer);

        public static TObject ParseAs<TObject>(this object @object, JsonSerializer jsonSerializer)
            => JToken.FromObject(@object, jsonSerializer).ToObject<TObject>(jsonSerializer);
    }
}
