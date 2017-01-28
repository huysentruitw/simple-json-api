using System;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal interface IDocumentBuilder
    {
        Document BuildDocument(object instance, Type type, JsonApiConfiguration configuration);
    }
}
