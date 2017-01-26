using System;
using Newtonsoft.Json.Linq;

namespace SimpleJsonApi.Models
{
    internal sealed class DocumentData
    {
        public Guid? Id { get; set; }

        public string Type { get; set; }

        public JObject Attributes { get; set; }

        public Relationships Relationships { get; set; }
    }
}
