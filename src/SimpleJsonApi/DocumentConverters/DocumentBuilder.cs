using System;
using System.Web.Http;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Extensions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal sealed class DocumentBuilder : IDocumentBuilder
    {
        public Document BuildDocument(object instance, Type type, JsonApiConfiguration configuration)
        {
            var httpError = instance as HttpError;
            if (httpError != null) return SerializeHttpError(httpError);

            throw new NotImplementedException();
        }

        private static Document SerializeHttpError(HttpError httpError)
        {
            return new Document { Errors = httpError.ToErrors() };
        }
    }
}
