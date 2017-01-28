using System.Reflection;

namespace SimpleJsonApi.Configuration.Internal
{
    internal sealed class IdPropertyInfo
    {
        public IdPropertyInfo(string name, PropertyInfo property)
        {
            Name = name;
            Property = property;
        }

        public string Name { get; }

        public PropertyInfo Property { get; }
    }
}
