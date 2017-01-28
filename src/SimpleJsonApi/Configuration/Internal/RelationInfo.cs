using System;
using System.Reflection;

namespace SimpleJsonApi.Configuration.Internal
{
    internal sealed class RelationInfo
    {
        public RelationInfo(PropertyInfo propertyInfo, Type relatedResourceType, RelationKind kind)
        {
            PropertyInfo = propertyInfo;
            ResourceType = relatedResourceType;
            Kind = kind;
        }

        public PropertyInfo PropertyInfo { get; }

        public Type ResourceType { get; }

        public RelationKind Kind { get; }
    }
}
