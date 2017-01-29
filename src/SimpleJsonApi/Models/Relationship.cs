using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleJsonApi.Models
{
    internal sealed class Relationship
    {
        [Obsolete("Constructor for serializer", true)]
        public Relationship()
        {
        }

        public Relationship(string typeName, Guid id)
        {
            Data = new RelationData
            {
                Id = id,
                Type = typeName
            };
        }

        public Relationship(string typeName, IEnumerable<Guid> ids)
        {
            Data = ids.Select(x => new RelationData
            {
                Id = x,
                Type = typeName
            });
        }

        /// <summary>
        /// Data is either a <see cref="RelationData"/> or <see cref="IEnumerable{RelationData}"/> instance.
        /// </summary>
        public object Data { get; set; }
    }
}
