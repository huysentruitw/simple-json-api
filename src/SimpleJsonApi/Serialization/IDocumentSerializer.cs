using System;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Serialization
{
    internal interface IDocumentSerializer
    {
        Document Serialize(object instance, Type type, JsonApiConfiguration configuration);
    }
}
