using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SimpleJsonApi.Models
{
    internal sealed class DocumentData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }

        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Attributes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, Relationship> Relationships { get; set; }
    }
}
