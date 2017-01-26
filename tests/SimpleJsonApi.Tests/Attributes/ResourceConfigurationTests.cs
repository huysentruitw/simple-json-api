using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SimpleJsonApi.Configuration;

namespace SimpleJsonApi.Tests.Attributes
{
    [TestFixture]
    public class ResourceConfigurationTests
    {
        [Test]
        public void ResourceConfiguration_GetMappingForType_ShouldReturnNullForUnknownMapping()
        {
            var mapping = new Dictionary<Type, IResourceMapping>
            {
                { typeof(ResourceA), new Mock<IResourceMapping>().Object },
                { typeof(ResourceC), new Mock<IResourceMapping>().Object }
            };

            var config = new ResourceConfiguration(mapping);
            Assert.That(config.GetMappingForType(typeof(ResourceB)), Is.Null);
        }

        public void ResourceConfiguration_GetMappingForType_ShouldReturnKnownMapping()
        {
            var mapping = new Dictionary<Type, IResourceMapping>
            {
                { typeof(ResourceA), new Mock<IResourceMapping>().Object },
                { typeof(ResourceC), new Mock<IResourceMapping>().Object }
            };

            var config = new ResourceConfiguration(mapping);
            Assert.That(config.GetMappingForType(typeof(ResourceA)), Is.EqualTo(mapping[typeof(ResourceA)]));
            Assert.That(config.GetMappingForType(typeof(ResourceC)), Is.EqualTo(mapping[typeof(ResourceC)]));
        }

        private class ResourceA { }
        private class ResourceB { }
        private class ResourceC { }
    }
}
