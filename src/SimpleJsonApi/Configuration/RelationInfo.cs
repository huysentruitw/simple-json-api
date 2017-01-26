using System;
using System.Reflection;

namespace SimpleJsonApi.Configuration
{
    internal sealed class RelationInfo
    {
        public RelationInfo(PropertyInfo propertyInfo, Type relatedResourceType, bool hasMany)
        {
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));
            if (relatedResourceType == null) throw new ArgumentNullException(nameof(relatedResourceType));
            PropertyInfo = propertyInfo;
            ResourceType = relatedResourceType;
            HasMany = hasMany;
        }

        public PropertyInfo PropertyInfo { get; }

        public Type ResourceType { get; }

        public bool HasMany { get; }
    }
}
