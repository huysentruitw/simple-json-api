using NUnit.Framework;
using SimpleJsonApi.Configuration.Internal;

namespace SimpleJsonApi.Tests.Configuration.Internal
{
    [TestFixture]
    public class RelationInfoTests
    {
        [Test]
        public void RelationInfo_Constructor_ShouldSetProperties()
        {
            var info = new RelationInfo(
                typeof(Resource).GetProperty(nameof(Resource.Age)),
                typeof(Resource),
                RelationKind.HasMany);

            Assert.That(info.PropertyInfo, Is.EqualTo(typeof(Resource).GetProperty(nameof(Resource.Age))));
            Assert.That(info.ResourceType, Is.EqualTo(typeof(Resource)));
            Assert.That(info.Kind, Is.EqualTo(RelationKind.HasMany));
        }

        private class Resource
        {
            public int Age { get; set; }
        }
    }
}
