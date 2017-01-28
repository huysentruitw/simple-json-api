using System;
using System.Web.Http;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Extensions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal sealed class DocumentBuilder : IDocumentBuilder
    {
        private readonly JsonApiConfiguration _configuration;

        public DocumentBuilder(JsonApiConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
        }

        public Document BuildDocument(object instance, Type type)
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
