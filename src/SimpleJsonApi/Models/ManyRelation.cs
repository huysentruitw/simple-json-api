using System.Collections.Generic;

namespace SimpleJsonApi.Models
{
    internal sealed class ManyRelation
    {
        public IEnumerable<RelationData> Data { get; set; }
    }
}
