using System;
using System.Collections.Generic;

namespace SimpleJsonApi.Configuration
{
    internal interface IResourceMapping
    {
        void SetId(object instance, Guid id);

        Type GetPropertyType(string propertyName);

        void SetProperty(object instance, string propertyName, object propertyValue);

        object GetProperty(object instance, string propertyName);

        Type GetResourceTypeOfRelation(string propertyName);

        bool HasManyRelation(string propertyName);

        void SetRelation(object instance, string propertyName, Guid relationId);

        void SetRelations(object instance, string propertyName, IEnumerable<Guid> relationIds);
    }
}
