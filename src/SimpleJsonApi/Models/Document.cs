using System.Collections.Generic;
using Newtonsoft.Json;

namespace SimpleJsonApi.Models
{
    internal sealed class Document
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DocumentData Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Error> Errors { get; set; }
    }
}
