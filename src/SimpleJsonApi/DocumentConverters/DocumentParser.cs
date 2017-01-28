﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Exceptions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal sealed class DocumentParser : IDocumentParser
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> BuilderCache = new ConcurrentDictionary<Type, MethodInfo>();
        private readonly JsonApiConfiguration _configuration;

        public DocumentParser(JsonApiConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
        }

        public object ParseDocument(Document document, Type type)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var isGenericChangesObject = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Changes<>);
            if (isGenericChangesObject) type = type.GenericTypeArguments.First();

            ValidateResourceType(document, type);
            var mapping = _configuration.ResourceConfiguration.GetMappingForType(type);
            if (mapping == null) throw new JsonApiException(CausedBy.Client, $"No mapping found for resource type {type.Name}");

            var builder = BuilderCache.GetOrAdd(type, t => typeof(DocumentParser).GetMethod(nameof(BuildChanges))?.MakeGenericMethod(t));
            if (builder == null) throw new JsonApiException(CausedBy.Server, $"Failed to create builder method for type {type.Name}");

            var changes = builder.Invoke(this, new object[] { document, mapping }) as IChanges;
            if (changes == null) throw new JsonApiException(CausedBy.Server, $"Builder method did not generate a class that inherits from {nameof(IChanges)}");

            if (isGenericChangesObject) return changes;

            var result = Activator.CreateInstance(type);
            changes.ApplyTo(result);
            return result;
        }

        [Obsolete("Only used by reflection inside Deserialize method", true)]
        public Changes<TResource> BuildChanges<TResource>(Document document, IResourceMapping mapping)
        {
            var changes = new Changes<TResource>();

            if (document.Data.Id.HasValue)
            {
                var id = document.Data.Id.Value;
                changes.AddChange(resource => mapping.SetId(resource, id));
            }

            if (document.Data.Attributes != null)
            {
                foreach (var attribute in document.Data.Attributes)
                {
                    var propertyType = mapping.GetPropertyType(attribute.Key);
                    if (propertyType == null) continue;
                    changes.AddChange(resource => mapping.SetProperty(resource, attribute.Key, attribute.Value.ToObject(propertyType)));
                }
            }

            if (document.Data.Relationships?.Items != null)
            {
                foreach (var relation in document.Data.Relationships.Items)
                {
                    var relationResourceType = mapping.GetResourceTypeOfRelation(relation.Key);
                    if (relationResourceType == null) throw new JsonApiException(CausedBy.Client, $"Unknown relation {relation.Key}");
                    var relationResourceTypeName = GetResourceTypeName(relationResourceType);
                    if (mapping.HasManyRelation(relation.Key))
                    {
                        var relationData = relation.Value.ToObject<ManyRelation>()?.Data;
                        if (relationData != null)
                        {
                            if (relationData.Any(x => !x.Type.Equals(relationResourceTypeName)))
                                throw new JsonApiFormatException(CausedBy.Client, $"Not all relations specified for {relation.Key} are of the type {relationResourceTypeName}");
                            var relationIds = relationData.Select(x => x.Id);
                            changes.AddChange(resource => mapping.SetRelations(resource, relation.Key, relationIds));
                        }
                    }
                    else
                    {
                        var relationData = relation.Value.ToObject<SingleRelation>()?.Data;
                        if (relationData != null)
                        {
                            if (!relationData.Type.Equals(relationResourceTypeName))
                                throw new JsonApiFormatException(CausedBy.Client, $"Relation type specified for {relation.Key} must be {relationResourceTypeName} instead of {relationData.Type}");
                            var relationId = relationData.Id;
                            changes.AddChange(resource => mapping.SetRelation(resource, relation.Key, relationId));
                        }
                    }
                }
            }

            return changes;
        }

        private string GetResourceTypeName(Type type)
            => _configuration.ResourceConfiguration.GetResourceTypeName(type);

        private void ValidateResourceType(Document document, Type type)
        {
            var destinationResourceType = GetResourceTypeName(type);
            if (!document.Data.Type.Equals(destinationResourceType, StringComparison.OrdinalIgnoreCase))
                throw new JsonApiException(CausedBy.Client, $"Invalid resource type {document.Data.Type} (should be {destinationResourceType})");
        }
    }
}