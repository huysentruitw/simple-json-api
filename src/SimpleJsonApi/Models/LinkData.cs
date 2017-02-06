using Newtonsoft.Json;

namespace SimpleJsonApi.Models
{
    internal sealed class LinkData
    {
        public string Href { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Meta { get; set; }
    }
}
