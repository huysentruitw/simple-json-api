using System;
using Newtonsoft.Json;

namespace SimpleJsonApi.Sample.Models
{
    public class Driver
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Licensed { get; set; }
        [JsonProperty("car")]
        public Guid CarId { get; set; }
    }
}