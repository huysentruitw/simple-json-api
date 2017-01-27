using System;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Serialization
{
    internal sealed class DocumentSerializer : IDocumentSerializer
    {
        public Document Serialize(object instance, Type type, JsonApiConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
}
