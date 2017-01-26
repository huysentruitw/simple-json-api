using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleJsonApi.Models
{
    internal sealed class Relationships
    {
        [JsonExtensionData]
        public IDictionary<string, JToken> Items { get; set; }
    }
}