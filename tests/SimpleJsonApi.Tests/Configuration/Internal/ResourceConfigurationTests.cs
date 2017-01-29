using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using SimpleJsonApi.Configuration.Internal;

namespace SimpleJsonApi.Tests.Configuration.Internal
{
    [TestFixture]
    public class ResourceConfigurationTests
    {
        [Test]
        public void ResourceConfiguration_Constructor_ShouldSetParameters()
        {
            var mapping = new ResourceMapping<ResourceA>(
                new Dictionary<string, PropertyInfo>(),
                new Dictionary<string, RelationInfo>(),
                new IdPropertyInfo("IdName", null));
            var info = new ResourceConfiguration<ResourceA>("SomeTypeName", mapping);

            Assert.That(info.ResourceType, Is.EqualTo(typeof(ResourceA)));
            Assert.That(info.TypeName, Is.EqualTo("SomeTypeName"));
            Assert.That(info.Mapping, Is.EqualTo(mapping));
        }

        private class ResourceA { }
    }
}
