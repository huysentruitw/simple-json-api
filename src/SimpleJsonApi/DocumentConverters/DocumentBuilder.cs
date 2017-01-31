using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Exceptions;
using SimpleJsonApi.Extensions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal sealed class DocumentBuilder : IDocumentBuilder
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> RelationshipBuilderCache = new ConcurrentDictionary<Type, MethodInfo>();
        private readonly IResourceConfigurations _resourceConfigurations;

        public DocumentBuilder(IResourceConfigurations resourceConfigurations)
        {
            _resourceConfigurations = resourceConfigurations;
        }

        public Document BuildDocument(object instance, Uri requestUri)
        {
            var httpError = instance as HttpError;
            if (httpError != null) return SerializeHttpError(httpError);

            var baseUri = requestUri.GetAbsoluteBaseUri();
            var includes = new HashSet<DocumentData>();

            return new Document
            {
                Links = BuildLinks(requestUri),
                Data = BuildData(instance, includes),
                Included = includes.Any() ? includes : null
            };
        }

        private IDictionary<string, string> BuildLinks(Uri requestUri)
        {
            return new Dictionary<string, string>
            {
                { "Self", requestUri.AbsoluteUri }
            };
        }

        private object BuildData(object instance, ISet<DocumentData> includes)
            => instance is IEnumerable
                    ? from object resource in (IEnumerable) instance select BuildSingleDataObject(resource, includes)
                    : (object)BuildSingleDataObject(instance, includes);

        private DocumentData BuildSingleDataObject(object instance, ISet<DocumentData> includes)
        {
            var resourceType = instance.GetType();
            var resourceConfiguration = _resourceConfigurations[resourceType];
            if (resourceConfiguration == null)
                throw new JsonApiException(CausedBy.Server, $"No configuration found for resource type {resourceType.Name}");

            var mapping = resourceConfiguration.Mapping;
            return new DocumentData
            {
                Id = mapping.GetId(instance),
                Type = resourceConfiguration.TypeName,
                Attributes = BuildAttributes(instance, mapping),
                Relationships = BuildRelationships(instance, mapping, includes)
            };
        }

        private IDictionary<string, object> BuildAttributes(object instance, IResourceMapping mapping)
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

        private IDictionary<string, Relationship> BuildRelationships(object instance, IResourceMapping mapping, ISet<DocumentData> includes)
        {
            var relationNames = mapping.GetRelationNames().ToList();
            if (!relationNames.Any()) return null;

            var relationships = new Dictionary<string, Relationship>();
            foreach (var relationName in relationNames)
            {
                var relationType = mapping.GetResourceTypeOfRelation(relationName);
                if (relationType == null) throw new JsonApiException(CausedBy.Server, $"Resource type for relation {relationName} is not configured");
                var relationTypeName = _resourceConfigurations[relationType]?.TypeName;

                var value = mapping.GetRelationValue(instance, relationName);
                if (value == null) continue;

                var builder = RelationshipBuilderCache.GetOrAdd(relationType,
                    t => typeof(DocumentBuilder).GetMethod(nameof(BuildRelationship), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(t));
                if (builder == null) throw new JsonApiException(CausedBy.Server, $"Failed to create generic method for type {relationType.Name}");

                var relationship = (Relationship)builder.Invoke(this, new[] { relationTypeName, value, includes });
                if (relationship != null) relationships.Add(relationName, relationship);
            }

            return relationships;
        }

        private Relationship BuildRelationship<TRelatedResource>(string typeName, object value, ISet<DocumentData> includes)
        {
            if (value is Guid) return new Relationship(typeName, (Guid)value);
            if (value is IEnumerable<Guid>) return new Relationship(typeName, (IEnumerable<Guid>)value);

            var mapping = _resourceConfigurations[typeof(TRelatedResource)].Mapping;
            if (value is TRelatedResource)
            {
                var documentData = BuildSingleDataObject(value, includes);
                includes.Add(documentData);
                return new Relationship(typeName, documentData.Id.Value);
            }

            if (value is IEnumerable<TRelatedResource>)
            {
                var documentDatas = ((IEnumerable<TRelatedResource>)value).Select(x => BuildSingleDataObject(x, includes));
                foreach (var documentData in documentDatas) includes.Add(documentData);
                return new Relationship(typeName, documentDatas.Select(x => x.Id.Value));
            }

            return null;
        }

        private static Document SerializeHttpError(HttpError httpError) => new Document { Errors = httpError.ToErrors() };
    }
}
