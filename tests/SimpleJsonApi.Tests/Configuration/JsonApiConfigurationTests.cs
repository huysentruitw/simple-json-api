using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using SimpleJsonApi.Configuration;

namespace SimpleJsonApi.Tests.Attributes
{
    [TestFixture]
    public class JsonApiConfigurationTests
    {
        [Test]
        public void JsonApiConfiguration_Constructur_VerifyConverters()
        {
            var config = new JsonApiConfiguration();
            Assert.That(config.SerializerSettings, Is.Not.Null);
            Assert.That(config.SerializerSettings.Converters.Any(x => x is IsoDateTimeConverter));
            Assert.That(config.SerializerSettings.Converters.Any(x => x is StringEnumConverter && ((StringEnumConverter)x).CamelCaseText));
        }

        [Test]
        public void JsonApiConfiguration_Validate_NoResourceConfiguration_ShouldThrowArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new JsonApiConfiguration().Validate());
            Assert.That(ex.ParamName, Is.EqualTo("ResourceConfiguration"));
        }

        [Test]
        public void JsonApiConfiguration_Validate_NoResourceConfiguration_DoesNotThrow()
        {
            var config = new JsonApiConfiguration
            {
                ResourceConfiguration = new ResourceConfiguration(new Dictionary<Type, IResourceMapping>())
            };

            Assert.DoesNotThrow(() => config.Validate());
        }
    }
}
