using System;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal interface IDocumentParser
    {
        object ParseDocument(Document document, Type type, JsonApiConfiguration configuration);
    }
}
