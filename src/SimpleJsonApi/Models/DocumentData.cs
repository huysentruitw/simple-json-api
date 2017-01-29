using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SimpleJsonApi.Models
{
    internal sealed class DocumentData
    {
        public Guid? Id { get; set; }

        public string Type { get; set; }

        public IDictionary<string, object> Attributes { get; set; }

        public IDictionary<string, Relationship> Relationships { get; set; }
    }
}
