using System;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Serialization
{
    internal interface IDocumentDeserializer
    {
        object Deserialize(Document document, Type type, JsonApiConfiguration configuration);
    }
}
