using System;
using NUnit.Framework;
using SimpleJsonApi.Attributes;
using SimpleJsonApi.Configuration;

namespace SimpleJsonApi.Tests.Configuration
{
    [TestFixture]
    public class ResourceConfigurationsBuilderTests
    {
        [Test]
        public void ResourceConfigurationsBuilder_Resource_DontPassName_ShouldGeneratePluralizedTypeName()
        {
            var builder = new ResourceConfigurationsBuilder();

            builder.Resource<User>();
            var config = builder.Build();
            var type = config[typeof(User)];

            Assert.That(type.TypeName, Is.EqualTo("users"));
        }

        [Test]
        public void ResourceConfigurationsBuilder_ResourceWithNameAttribute_ShouldGeneratePluralizedTypeName()
        {
            var builder = new ResourceConfigurationsBuilder();

            builder.Resource<ResourceWithAttribute>();
            var config = builder.Build();
            var type = config[typeof(ResourceWithAttribute)];

            Assert.That(type.TypeName, Is.EqualTo("cars"));
        }

        [Test]
        public void ResourceConfigurationsBuilder_Resource_PassCustomName_ShouldUsePassedTypeNameAsIs()
        {
            var builder = new ResourceConfigurationsBuilder();

            builder.Resource<ResourceWithAttribute>("driver");
            var config = builder.Build();
            var type = config[typeof(ResourceWithAttribute)];

            Assert.That(type.TypeName, Is.EqualTo("driver"));
        }

        private class User
        {
            public Guid Id { get; set; }
        }

        [JsonApiResourceName("car")]
        private class ResourceWithAttribute
        {
            public Guid Id { get; set; }
        }
    }
}
