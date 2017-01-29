using System;

namespace SimpleJsonApi.Configuration
{
    /// <summary>
    /// Keeps track of the info and mapping of a single resource.
    /// </summary>
    public interface IResourceConfiguration
    {
        /// <summary>
        /// The type of the resource. This is the type you want to convert to/from a JSON API document.
        /// </summary>
        Type ResourceType { get; }

        /// <summary>
        /// When using the <see cref="ResourceConfigurationsBuilder"/> to generate this configuration,
        /// this type is automatically derived from the resource type. F.e. if the type is 'Car', the <see cref="TypeName"/> will be 'cars'.<br />
        /// To override the generated name, use the <see cref="SimpleJsonApi.Attributes.JsonApiResourceNameAttribute"/> on the resource definition
        /// or pass a name to the Resource method of <see cref="ResourceConfigurationBuilder{TResource}"/>.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// The mapping object for this resource.
        /// </summary>
        IResourceMapping Mapping { get; }
    }
}
