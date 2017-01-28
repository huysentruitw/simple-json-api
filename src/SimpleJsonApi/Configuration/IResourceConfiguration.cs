using System;

namespace SimpleJsonApi.Configuration
{
    public interface IResourceConfiguration
    {
        Type ResourceType { get; }

        string TypeName { get; }

        IResourceMapping Mapping { get; }
    }
}
