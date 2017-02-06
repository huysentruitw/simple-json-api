﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Exceptions;
using SimpleJsonApi.Extensions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.DocumentConverters
{
    internal sealed class DocumentParser : IDocumentParser
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> BuilderCache = new ConcurrentDictionary<Type, MethodInfo>();
        private readonly IResourceConfigurations _resourceConfigurations;
        private readonly JsonSerializer _jsonSerializer;

        public DocumentParser(IResourceConfigurations resourceConfigurations, JsonSerializer jsonSerializer)
        {
            _resourceConfigurations = resourceConfigurations;
            _jsonSerializer = jsonSerializer;
        }

        public object ParseDocument(Document document, Type type)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var isGenericChangesObject = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Changes<>);
            if (isGenericChangesObject) type = type.GenericTypeArguments.First();

            var documentData = document.Data?.ParseAs<DocumentData>(_jsonSerializer);
            if (documentData == null) throw new JsonApiException(CausedBy.Client, $"Document data not found or invalid");

            ValidateResourceType(documentData, type);
            var mapping = _resourceConfigurations[type]?.Mapping;
            if (mapping == null) throw new JsonApiException(CausedBy.Client, $"No mapping found for resource type {type.Name}");

            var builder = BuilderCache.GetOrAdd(type, t => typeof(DocumentParser).GetMethod(nameof(BuildChanges), BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(t));
            if (builder == null) throw new JsonApiException(CausedBy.Server, $"Failed to create builder method for type {type.Name}");

            var changes = builder.Invoke(this, new object[] { documentData, mapping }) as IChanges;
            if (changes == null) throw new JsonApiException(CausedBy.Server, $"Builder method did not generate a class that inherits from {nameof(IChanges)}");

            if (isGenericChangesObject) return changes;

            var result = Activator.CreateInstance(type);
            changes.ApplyTo(result);
            return result;
        }

        private Changes<TResource> BuildChanges<TResource>(DocumentData data, IResourceMapping mapping)
        {
            var changes = new Changes<TResource>();

            if (data.Id.HasValue)
            {
                changes.AddChange(resource => mapping.SetId(resource, data.Id.Value));
            }

            if (data.Attributes != null)
            {
                foreach (var attribute in data.Attributes)
                {
                    var attributeType = mapping.GetAttributeType(attribute.Key);
                    if (attributeType == null) continue;
                    var value = attribute.Value.ParseAs(attributeType, _jsonSerializer);
                    changes.AddChange(resource => mapping.SetAttributeValue(resource, attribute.Key, value));
                }
            }

            if (data.Relationships != null)
            {
                foreach (var relation in data.Relationships)
                {
                    var relationResourceType = mapping.GetResourceTypeOfRelation(relation.Key);
                    if (relationResourceType == null) throw new JsonApiException(CausedBy.Client, $"Unknown relation {relation.Key}");
                    var relationResourceTypeName = GetResourceTypeName(relationResourceType);
                    if (mapping.IsHasManyRelation(relation.Key))
                    {
                        var relationData = relation.Value.Data.ParseAs<IEnumerable<RelationData>>(_jsonSerializer);
                        if (relationData != null)
                        {
                            if (relationData.Any(x => !x.Type.Equals(relationResourceTypeName)))
                                throw new JsonApiFormatException(CausedBy.Client, $"Not all relations specified for {relation.Key} are of the type {relationResourceTypeName}");
                            var relationIds = relationData.Select(x => x.Id);
                            changes.AddChange(resource => mapping.SetRelationValue(resource, relation.Key, relationIds));
                        }
                    }
                    else
                    {
                        var relationData = relation.Value.Data.ParseAs<RelationData>(_jsonSerializer);
                        if (relationData != null)
                        {
                            if (!relationData.Type.Equals(relationResourceTypeName))
                                throw new JsonApiFormatException(CausedBy.Client, $"Relation type specified for {relation.Key} must be {relationResourceTypeName} instead of {relationData.Type}");
                            var relationId = relationData.Id;
                            changes.AddChange(resource => mapping.SetRelationValue(resource, relation.Key, relationId));
                        }
                    }
                }
            }

            return changes;
        }

        private string GetResourceTypeName(Type type) => _resourceConfigurations[type]?.TypeName;

        private void ValidateResourceType(DocumentData data, Type type)
        {
            var destinationResourceType = GetResourceTypeName(type);
            if (!data.Type.Equals(destinationResourceType, StringComparison.OrdinalIgnoreCase))
                throw new JsonApiException(CausedBy.Client, $"Invalid resource type {data.Type} (should be {destinationResourceType})");
        }
    }
}
