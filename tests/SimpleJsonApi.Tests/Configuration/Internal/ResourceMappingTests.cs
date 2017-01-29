using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SimpleJsonApi.Configuration.Internal;

namespace SimpleJsonApi.Tests.Configuration.Internal
{
    [TestFixture]
    public class ResourceMappingTests
    {
        private Resource _resource;
        private ResourceMapping<Resource> _mappingUnderTest;

        [SetUp]
        public void SetUp()
        {
            var someRelation = new RelationInfo(
                typeof(Resource).GetProperty(nameof(Resource.SomeRelation)),
                typeof(RelatedResource),
                RelationKind.BelongsTo);

            var manyRelation = new RelationInfo(
                typeof(Resource).GetProperty(nameof(Resource.ManyRelation)),
                typeof(RelatedResource),
                RelationKind.HasMany);

            _resource = new Resource();

            _mappingUnderTest = new ResourceMapping<Resource>(
                new Dictionary<string, PropertyInfo>
                {
                    { "name", typeof(Resource).GetProperty(nameof(Resource.Name)) },
                    { "age", typeof(Resource).GetProperty(nameof(Resource.Age)) },
                }, 
                new Dictionary<string, RelationInfo>
                {
                    { "someRelation", someRelation },
                    { "manyRelation", manyRelation }
                }, 
                new IdPropertyInfo("myid", typeof(Resource).GetProperty(nameof(Resource.MyId))));
        }

        [Test]
        public void ResourceMapping_SetId_ShouldSetIdOnInstance()
        {
            var testId = Guid.NewGuid();
            _mappingUnderTest.SetId(_resource, testId);
            Assert.That(_resource.MyId, Is.EqualTo(testId));
        }

        [Test]
        public void ResourceMapping_GetId_ShouldReturnCorrectId()
        {
            var testId = Guid.NewGuid();
            _resource.MyId = testId;
            Assert.That(_mappingUnderTest.GetId(_resource), Is.EqualTo(testId));
        }

        [Test]
        public void ResourceMapping_GetAttributeNames_ShouldReturnAttributeNames()
        {
            var names = _mappingUnderTest.GetAttributeNames().ToList();
            Assert.That(names.Count, Is.EqualTo(2));
            Assert.That(names, Contains.Item("name"));
            Assert.That(names, Contains.Item("age"));
        }

        [Test]
        public void ResourceMapping_GetAttributeType_ShouldReturnAttributeTypeOrNull()
        {
            Assert.That(_mappingUnderTest.GetAttributeType("name"), Is.EqualTo(typeof(string)));
            Assert.That(_mappingUnderTest.GetAttributeType("age"), Is.EqualTo(typeof(int)));
            Assert.That(_mappingUnderTest.GetAttributeType("unknown"), Is.Null);
        }

        [Test]
        public void ResourceMapping_SetAttributeValue_ShouldSetAttributeValue()
        {
            var name = Guid.NewGuid().ToString("N");
            _mappingUnderTest.SetAttributeValue(_resource, "name", name);
            Assert.That(_resource.Name, Is.EqualTo(name));
        }

        [Test]
        public void ResourceMapping_GetAttributeValue_ShouldReturnCorrectValueOrNull()
        {
            _resource.Age = 111;
            Assert.That(_mappingUnderTest.GetAttributeValue(_resource, "age"), Is.EqualTo(111));
            _resource.Age = 66;
            Assert.That(_mappingUnderTest.GetAttributeValue(_resource, "age"), Is.EqualTo(66));
            Assert.That(_mappingUnderTest.GetAttributeValue(_resource, "unknown"), Is.Null);
        }

        [Test]
        public void ResourceMapping_GetRelationNames_ShouldReturnRelationNames()
        {
            var names = _mappingUnderTest.GetRelationNames().ToList();
            Assert.That(names.Count, Is.EqualTo(2));
            Assert.That(names, Contains.Item("someRelation"));
            Assert.That(names, Contains.Item("manyRelation"));
        }

        [Test]
        public void ResourceMapping_GetResourceTypeOfRelation_ShouldReturnRelationTypeOrNull()
        {
            Assert.That(_mappingUnderTest.GetResourceTypeOfRelation("someRelation"), Is.EqualTo(typeof(RelatedResource)));
            Assert.That(_mappingUnderTest.GetResourceTypeOfRelation("otherRelation"), Is.Null);
        }

        [Test]
        public void ResourceMapping_IsHasManyRelation_ShouldReturnCorrectRelationKind()
        {
            Assert.That(_mappingUnderTest.IsHasManyRelation("someRelation"), Is.False);
            Assert.That(_mappingUnderTest.IsHasManyRelation("manyRelation"), Is.True);
        }

        [Test]
        public void ResourceMapping_SetSingleRelationValue_ShouldSetRelationValue()
        {
            var testId = Guid.NewGuid();
            _resource.SomeRelation = Guid.NewGuid();
            _mappingUnderTest.SetRelationValue(_resource, "someRelation", testId);
            Assert.That(_resource.SomeRelation, Is.EqualTo(testId));
        }

        [Test]
        public void ResourceMapping_SetManyRelationValue_ShouldSetRelationValue()
        {
            var testIds = Enumerable.Range(1, 10).Select(x => Guid.NewGuid());
            _resource.ManyRelation = null;
            _mappingUnderTest.SetRelationValue(_resource, "manyRelation", testIds);
            Assert.That(_resource.ManyRelation, Is.EqualTo(testIds));
        }

        [Test]
        public void ResourceMapping_GetRelationValue_ShouldGetRelationValueOrNull()
        {
            var testId = Guid.NewGuid();
            _resource.SomeRelation = testId;
            Assert.That(_mappingUnderTest.GetRelationValue(_resource, "someRelation"), Is.EqualTo(testId));

            Assert.That(_mappingUnderTest.GetRelationValue(_resource, "otherRelation"), Is.Null);
        }

        private class Resource
        {
            public Guid MyId { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public Guid SomeRelation { get; set; }
            public IEnumerable<Guid> ManyRelation { get; set; }
        }

        private class RelatedResource { }
    }
}
