using System;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Serialization
{
    internal interface IDocumentDeserializer
    {
        object Deserialize(UpdateDocument document, Type type, JsonApiConfiguration configuration);
    }
}
