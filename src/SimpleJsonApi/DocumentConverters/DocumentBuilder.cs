using System;
using System.Collections.Generic;
using System.Linq;
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
                Links = GenerateLinks(requestUri),
                Data = GenerateData(instance, resourceConfiguration)
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

        private DocumentData GenerateData(object instance, IResourceConfiguration configuration)
        {
            var mapping = configuration.Mapping;
            return new DocumentData
            {
                Id = mapping.GetId(instance),
                Type = configuration.TypeName,
                Attributes = GenerateAttributes(instance, mapping),
                Relationships = GenerateRelationships(instance, mapping)
            };
        }

        private IDictionary<string, object> GenerateAttributes(object instance, IResourceMapping mapping)
        {
            var attributeNames = mapping.GetAttributeNames().ToList();
            if (!attributeNames.Any()) return null;

            var attributes = new Dictionary<string, object>();
            foreach (var attributeName in attributeNames)
            {
                var value = mapping.GetAttributeValue(instance, attributeName);
                attributes.Add(attributeName, value);
            }

            return attributes;
        }

        private IDictionary<string, Relationship> GenerateRelationships(object instance, IResourceMapping mapping)
        {
            var relationNames = mapping.GetRelationNames().ToList();
            if (!relationNames.Any()) return null;

            var relationships = new Dictionary<string, Relationship>();
            foreach (var relationName in relationNames)
            {
                var relationTypeName = _configuration.ResourceConfigurations[mapping.GetResourceTypeOfRelation(relationName)]?.TypeName;
                if (relationTypeName == null) throw new JsonApiException(CausedBy.Server, $"Resource type for relation {relationName} is not configured");

                var value = mapping.GetRelationValue(instance, relationName);
                if (value == null) return null;

                if (value is Guid)
                {
                    relationships.Add(relationName, new Relationship(relationTypeName, (Guid)value));
                }
                else if (value is IEnumerable<Guid>)
                {
                    relationships.Add(relationName, new Relationship(relationTypeName, (IEnumerable<Guid>)value));
                }
            }

            return relationships;
        }

        private static Document SerializeHttpError(HttpError httpError)
        {
            return new Document { Errors = httpError.ToErrors() };
        }
    }
}
