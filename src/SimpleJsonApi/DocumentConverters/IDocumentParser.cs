using System;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal interface IDocumentParser
    {
        object ParseDocument(Document document, Type type);
    }
}
