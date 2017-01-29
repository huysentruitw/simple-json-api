using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Exceptions;
using SimpleJsonApi.Extensions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal sealed class DocumentBuilder : IDocumentBuilder
    {
        private readonly JsonApiConfiguration _configuration;
        private readonly JsonSerializer _jsonSerializer;

        public DocumentBuilder(JsonApiConfiguration configuration)
        {
            _configuration = configuration;
            _jsonSerializer = JsonSerializer.Create(configuration.SerializerSettings);
        }

        public Document BuildDocument(object instance, Type type, Uri requestUri)
        {
            var httpError = instance as HttpError;
            if (httpError != null) return SerializeHttpError(httpError);

            var resourceConfiguration = _configuration.ResourceConfigurations[type];
            if (resourceConfiguration == null) throw new JsonApiException(CausedBy.Server, $"No configuration found for resource type {type.Name}");

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
