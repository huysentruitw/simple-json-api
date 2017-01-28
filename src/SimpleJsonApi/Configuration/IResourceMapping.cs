using System;
using System.Collections.Generic;

namespace SimpleJsonApi.Configuration
{
    public interface IResourceMapping
    {
        void SetId(object instance, Guid id);

        Type GetAttributeType(string name);

        void SetAttributeValue(object instance, string name, object value);

        object GetAttributeValue(object instance, string name);

        Type GetResourceTypeOfRelation(string name);

        bool IsHasManyRelation(string name);

        void SetRelationValue(object instance, string name, Guid relationId);

        void SetRelationValues(object instance, string name, IEnumerable<Guid> relationIds);
    }
}
