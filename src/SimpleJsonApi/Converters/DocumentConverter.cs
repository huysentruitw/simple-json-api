using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Humanizer;
using SimpleJsonApi.Attributes;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Exceptions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Converters
{
    internal sealed class DocumentConverter
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> BuilderCache = new ConcurrentDictionary<Type, MethodInfo>();
        private readonly Document _document;
        private readonly JsonApiConfiguration _configuration;

        public DocumentConverter(Document document, JsonApiConfiguration configuration)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _document = document;
            _configuration = configuration;
        }

        public object ConvertTo(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var isGenericChangesObject = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Changes<>);
            if (isGenericChangesObject) type = type.GenericTypeArguments.First();

            ValidateResourceType(type);
            var mapping = _configuration.ResourceConfiguration.GetMappingForType(type);
            if (mapping == null) throw new JsonApiException($"No mapping found for resource type {type.Name}");

            var builder = BuilderCache.GetOrAdd(type, t => typeof(DocumentConverter).GetMethod(nameof(BuildChanges))?.MakeGenericMethod(t));
            if (builder == null) throw new JsonApiException($"Failed to create builder method for type {type.Name}");

            var changes = builder.Invoke(this, new object[] { mapping }) as IChanges;
            if (changes == null) throw new JsonApiException($"Builder method did not generate a class that inherits from {nameof(IChanges)}");

            if (isGenericChangesObject) return changes;

            var result = Activator.CreateInstance(type);
            changes.ApplyTo(result);
            return result;
        }

        public Changes<TResource> BuildChanges<TResource>(IResourceMapping mapping)
        {
            var changes = new Changes<TResource>();

            if (_document.Data.Id.HasValue)
            {
                var id = _document.Data.Id.Value;
                changes.AddChange(resource => mapping.SetId(resource, id));
            }

            if (_document.Data.Attributes != null)
            {
                foreach (var attribute in _document.Data.Attributes)
                {
                    var propertyType = mapping.GetPropertyType(attribute.Key);
                    if (propertyType == null) continue;
                    changes.AddChange(resource => mapping.SetProperty(resource, attribute.Key, attribute.Value.ToObject(propertyType)));
                }
            }

            if (_document.Data.Relationships?.Items != null)
            {
                foreach (var relation in _document.Data.Relationships.Items)
                {
                    var relationResourceType = mapping.GetResourceTypeOfRelation(relation.Key);
                    if (relationResourceType == null) throw new JsonApiException($"No relation defined for {relation.Key}");
                    var relationResourceTypeName = GetResourceTypeName(relationResourceType);
                    if (mapping.HasManyRelation(relation.Key))
                    {
                        var relationData = relation.Value.ToObject<ManyRelation>()?.Data;
                        if (relationData != null)
                        {
                            if (relationData.Any(x => !x.Type.Equals(relationResourceTypeName)))
                                throw new JsonApiFormatException($"Not all relations specified for {relation.Key} are of the type {relationResourceTypeName}");
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
                                throw new JsonApiFormatException($"Relation type specified for {relation.Key} must be {relationResourceTypeName} instead of {relationData.Type}");
                            var relationId = relationData.Id;
                            changes.AddChange(resource => mapping.SetRelation(resource, relation.Key, relationId));
                        }
                    }
                }
            }

            return changes;
        }

        private void ValidateResourceType(Type type)
        {
            var destinationResourceType = GetResourceTypeName(type);
            if (!_document.Data.Type.Equals(destinationResourceType, StringComparison.OrdinalIgnoreCase))
                throw new JsonApiException($"Can't convert resource type {_document.Data.Type} to {destinationResourceType}");
        }

        private string GetResourceTypeName(Type resourceType)
        {
            var nameAttribute = resourceType.GetCustomAttribute<JsonApiResourceNameAttribute>();
            var singularName = (nameAttribute?.Name ?? resourceType.Name).ToLower();
            return _configuration.PluralizeResourceTypeNames ? singularName.Pluralize() : singularName;
        }
    }
}
