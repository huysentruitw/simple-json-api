using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Configuration.Internal;

namespace SimpleJsonApi.Tests.Configuration
{
    [TestFixture]
    public class ResourceConfigurationBuilderTests
    {
        [Test]
        public void ResourceConfigurationBuilder_ZeroConfiguration_ShouldMapAllNonIdPropertiesAsAttributes()
        {
            IResourceConfigurationBuilder builder = new ResourceConfigurationBuilder<Resource>("resource");
            var config = builder.Build();

            Assert.That(config.TypeName, Is.EqualTo("resource"));
            Assert.That(config.ResourceType, Is.EqualTo(typeof(Resource)));
            var names = config.Mapping.GetAttributeNames().ToList();
            Assert.That(names.Count, Is.EqualTo(5));
            Assert.That(names, Does.Contain("OtherId"));
            Assert.That(names, Does.Contain("Name"));
            Assert.That(names, Does.Contain("Active"));
            Assert.That(names, Does.Contain("Parent"));
            Assert.That(names, Does.Contain("Children"));

            var testResource = new Resource();
            var testId = Guid.NewGuid();
            config.Mapping.SetId(testResource, testId);
            Assert.That(testResource.Id, Is.EqualTo(testId));

            Assert.That(testResource.Active, Is.False);
            config.Mapping.SetAttributeValue(testResource, "Active", true);
            Assert.That(testResource.Active, Is.True);
        }

        [Test]
        public void ResourceConfigurationBuilder_IgnoreProperty_ShouldExcludePropertyFromMapping()
        {
            IResourceConfigurationBuilder builder = new ResourceConfigurationBuilder<Resource>("resource")
                .IgnoreProperty(x => x.OtherId);
            var config = builder.Build();

            var names = config.Mapping.GetAttributeNames().ToList();
            Assert.That(names.Count, Is.EqualTo(4));
            Assert.That(names, Does.Not.Contain("OtherId"));
            Assert.That(config.Mapping.GetRelationNames().Count(), Is.Zero);
        }

        [Test]
        public void ResourceConfigurationBuilder_IgnoreIdProperty_ShouldThrowException()
        {
            IResourceConfigurationBuilder builder = new ResourceConfigurationBuilder<Resource>("resource")
                .IgnoreProperty(x => x.Id);
            var ex = Assert.Throws<Exception>(() => builder.Build());
            Assert.That(ex.Message, Does.Contain("No id property defined"));
        }

        [Test]
        public void ResourceConfigurationBuilder_ConfigureRelations_ShouldMapRelations()
        {
            IResourceConfigurationBuilder builder = new ResourceConfigurationBuilder<Resource>("resource")
                .BelongsTo<Parent>(x => x.Parent)
                .HasMany<Child>(x => x.Children);

            var config = builder.Build();
            Assert.That(config.Mapping.GetAttributeNames().Count, Is.EqualTo(3));

            var names = config.Mapping.GetRelationNames().ToList();
            Assert.That(names.Count, Is.EqualTo(2));

            var testResource = new Resource();
            var testId = Guid.NewGuid();
            config.Mapping.SetRelationValue(testResource, "Parent", testId);
            Assert.That(testResource.Parent, Is.EqualTo(testId));

            var testIds = new[] {Guid.NewGuid(), Guid.NewGuid()};
            config.Mapping.SetRelationValue(testResource, "Children", testIds);
            Assert.That(testResource.Children, Is.EqualTo(testIds));
        }

        [Test]
        public void ResourceConfigurationBuilder_ConfigureIdProperty_ShouldMapSelectedProperty()
        {
            IResourceConfigurationBuilder builder = new ResourceConfigurationBuilder<Resource>("resource")
                .WithIdProperty(x => x.OtherId);

            var config = builder.Build();
            Assert.That(config.Mapping.GetAttributeNames(), Does.Not.Contain("OtherId"));

            var testResource = new Resource();
            var testId = Guid.NewGuid();
            config.Mapping.SetId(testResource, testId);
            Assert.That(testResource.OtherId, Is.EqualTo(testId));
        }

        [Test]
        public void ResourceConfigurationBuilder_ConfigureNonGuidIdProperty_ShouldThrowException()
        {
            var builder = new ResourceConfigurationBuilder<Resource>("resource");
            var ex = Assert.Throws<ArgumentException>(() => builder.WithIdProperty(x => x.Name));
            Assert.That(ex.Message, Does.Contain("The id property must be of type Guid"));
        }

        private class Resource
        {
            public Guid Id { get; set; }
            public Guid OtherId { get; set; }
            public string Name { get; set; }
            public bool Active { get; set; }
            public Guid Parent { get; set; }
            public IEnumerable<Guid> Children { get; set; }
        }

        private class Parent { }

        private class Child { }
    }
}
