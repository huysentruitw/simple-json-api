using System;
using System.Net.Http;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal interface IDocumentBuilder
    {
        Document BuildDocument(object instance, HttpRequestMessage request);
    }
}
