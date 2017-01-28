using System;
using System.Collections.Generic;
using System.Web.Http;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Exceptions;
using SimpleJsonApi.Extensions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal sealed class DocumentBuilder : IDocumentBuilder
    {
        private readonly IResourceConfigurations _resourceConfigurations;

        public DocumentBuilder(IResourceConfigurations resourceConfigurations)
        {
            _resourceConfigurations = resourceConfigurations;
        }

        public Document BuildDocument(object instance, Type type, Uri requestUri)
        {
            var httpError = instance as HttpError;
            if (httpError != null) return SerializeHttpError(httpError);

            var mapping = _resourceConfigurations[type];
            if (mapping == null) throw new JsonApiException(CausedBy.Server, $"No mapping found for resource type {type.Name}");

            var document = new Document
            {
                Links = GenerateLinks(requestUri)
            };

            return document;
        }

        private IDictionary<string, string> GenerateLinks(Uri requestUri)
        {
            return new Dictionary<string, string>
            {
                { "Self", requestUri.AbsoluteUri }
            };
        }

        private static Document SerializeHttpError(HttpError httpError)
        {
            return new Document { Errors = httpError.ToErrors() };
        }
    }
}
