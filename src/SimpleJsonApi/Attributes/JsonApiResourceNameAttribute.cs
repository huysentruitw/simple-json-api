using System;

namespace SimpleJsonApi.Attributes
{
    /// <summary>
    /// Attribute to override the automatically generated JSON API resource name.
    /// </summary>
    /// <remarks>Singular word will be converted to its plural form.</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class JsonApiResourceNameAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The JSON API resource name.</param>
        public JsonApiResourceNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        /// <summary>
        /// The JSON API resource name as passed to the attribute.
        /// </summary>
        /// <remarks>Singular word will be converted to its plural form internally without reflecting this in this property.</remarks>
        public string Name { get; }
    }
}
