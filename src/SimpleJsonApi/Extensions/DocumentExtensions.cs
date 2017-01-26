using System;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Converters;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Extensions
{
    internal static class DocumentExtensions
    {
        public static object ConvertTo(this Document document, Type type, JsonApiConfiguration configuration)
        {
            return new DocumentConverter(document, configuration).ConvertTo(type);
        }
    }
}
