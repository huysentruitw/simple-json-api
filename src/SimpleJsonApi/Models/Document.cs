using System.Collections.Generic;
using Newtonsoft.Json;

namespace SimpleJsonApi.Models
{
    internal sealed class Document
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Links { get; set; }

        /// <summary>
        /// Data is either a <see cref="DocumentData"/> or <see cref="IEnumerable{DocumentData}"/> instance.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Error> Errors { get; set; }
    }
}
