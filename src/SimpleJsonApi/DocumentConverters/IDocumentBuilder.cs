using System;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal interface IDocumentBuilder
    {
        Document BuildDocument(object instance, Uri requestUri);
    }
}
