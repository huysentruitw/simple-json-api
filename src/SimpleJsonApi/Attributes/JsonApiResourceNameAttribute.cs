using System;

namespace SimpleJsonApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JsonApiResourceNameAttribute : Attribute
    {
        public JsonApiResourceNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public string Name { get; }
    }
}
