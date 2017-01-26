using NUnit.Framework;
using SimpleJsonApi.Configuration;

namespace SimpleJsonApi.Tests.Attributes
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
                true);

            Assert.That(info.PropertyInfo, Is.EqualTo(typeof(Resource).GetProperty(nameof(Resource.Age))));
            Assert.That(info.ResourceType, Is.EqualTo(typeof(Resource)));
            Assert.That(info.HasMany, Is.True);
        }

        private class Resource
        {
            public int Age { get; set; }
        }
    }
}
