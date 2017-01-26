using System;
using NUnit.Framework;
using SimpleJsonApi.Attributes;

namespace SimpleJsonApi.Tests.Attributes
{
    [TestFixture]
    public class JsonApiResourceNameAttributeTests
    {
        [Test]
        public void JsonApiResourceNameAttribute_Construct_ShouldSetNameProperty()
        {
            var name = Guid.NewGuid().ToString("N");
            var attribute = new JsonApiResourceNameAttribute(name);
            Assert.That(attribute.Name, Is.EqualTo(name));
        }
    }
}
