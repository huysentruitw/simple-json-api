using NUnit.Framework;
using SimpleJsonApi.Configuration.Internal;

namespace SimpleJsonApi.Tests.Configuration.Internal
{
    [TestFixture]
    public class IdPropertyInfoTests
    {
        [Test]
        public void IdPropertyInfo_Constructor_ShouldSetProperties()
        {
            var property = typeof(ResourceA).GetProperty(nameof(ResourceA.Property));
            var info = new IdPropertyInfo("Blabla", property);
            Assert.That(info.Name, Is.EqualTo("Blabla"));
            Assert.That(info.Property, Is.EqualTo(property));
        }

        private class ResourceA
        {
            public string Property { get; } = string.Empty;
        }
    }
}
