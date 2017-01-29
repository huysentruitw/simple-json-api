using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Configuration.Internal;

namespace SimpleJsonApi.Tests.Configuration.Internal
{
    [TestFixture]
    public class ResourceConfigurationsTests
    {
        [Test]
        public void ResourceConfigurations_Constructor_ShouldPassProperties()
        {
            var a = new Mock<IResourceConfiguration>().Object;
            var c = new Mock<IResourceConfiguration>().Object;

            var config = new Dictionary<Type, IResourceConfiguration>
            {
                { typeof(ResourceA), a }, { typeof(ResourceC), c }
            };

            var configs = new ResourceConfigurations(config);

            Assert.That(configs[typeof(ResourceA)], Is.EqualTo(a));
            Assert.That(configs[typeof(ResourceC)], Is.EqualTo(c));

            Assert.True(configs.Contains(typeof(ResourceA)));
            Assert.False(configs.Contains(typeof(ResourceB)));
            Assert.True(configs.Contains(typeof(ResourceC)));
        }

        [Test]
        public void ResourceConfigurations_Indexer_ShouldReturnNullForMissingResourceType()
        {
            var a = new Mock<IResourceConfiguration>().Object;
            var c = new Mock<IResourceConfiguration>().Object;

            var config = new Dictionary<Type, IResourceConfiguration>
            {
                { typeof(ResourceA), a }, { typeof(ResourceC), c }
            };

            var configs = new ResourceConfigurations(config);

            Assert.That(configs[typeof(ResourceA)], Is.Not.Null);
            Assert.That(configs[typeof(ResourceB)], Is.Null);
            Assert.That(configs[typeof(ResourceC)], Is.Not.Null);
        }

        private class ResourceA { }
        private class ResourceB { }
        private class ResourceC { }
    }
}
