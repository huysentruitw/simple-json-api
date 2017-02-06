using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleJsonApi.Extensions;

namespace SimpleJsonApi.Tests.Extensions
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void Implements_TestDifferentCombinations_ShouldGiveExpectedResult()
        {
            Assert.True(typeof(List<string>).Implements(typeof(IEnumerable<string>)));
            Assert.True(typeof(List<string>).Implements(typeof(IEnumerable<>)));
            Assert.True(typeof(List<string>).Implements(typeof(IEnumerable)));
            Assert.False(typeof(List<string>).Implements(typeof(IEnumerable<int>)));
            Assert.False(typeof(List<string>).Implements(typeof(IEqualityComparer<>)));
            Assert.True(typeof(Changes<Resource>).Implements(typeof(Changes<>)));
            Assert.True(typeof(Changes<Resource>).Implements(typeof(Changes<Resource>)));
            Assert.False(typeof(Changes<Resource>).Implements(typeof(Changes<OtherResource>)));
        }

        [Test]
        public void ToGenericIEnumerableVariant_TestDifferentBaseTypes_ShouldGiveExpectedResult()
        {
            Assert.That(typeof(string).ToGenericIEnumerableVariant(), Is.EqualTo(typeof(IEnumerable<string>)));
            Assert.That(typeof(Resource).ToGenericIEnumerableVariant(), Is.EqualTo(typeof(IEnumerable<Resource>)));
            Assert.That(typeof(Resource).ToGenericIEnumerableVariant(), Is.Not.EqualTo(typeof(IEnumerable<OtherResource>)));
        }

        [Test]
        public void GetFirstGenericArgument_PassIEnumerableResource_ShouldReturnResourceType()
        {
            Assert.That(typeof(IEnumerable<Resource>).GetFirstGenericArgument(), Is.EqualTo(typeof(Resource)));
        }

        private class Resource { }

        private class OtherResource { }
    }
}
