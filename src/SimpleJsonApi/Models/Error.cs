using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SimpleJsonApi.Models
{
    internal sealed class Error
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ErrorLinks Links { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Detail { get; set; }

        // Source

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Meta { get; set; }
    }

    internal sealed class ErrorLinks
    {
        public string About { get; set; }
    }
}
