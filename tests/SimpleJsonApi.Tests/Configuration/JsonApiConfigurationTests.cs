using System;
using System.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using SimpleJsonApi.Configuration;

namespace SimpleJsonApi.Tests.Configuration
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
        public void JsonApiConfiguration_Constructor_VerifyContractResolver()
        {
            var config = new JsonApiConfiguration();
            Assert.True(config.SerializerSettings.ContractResolver.GetType() == typeof(CamelCasePropertyNamesContractResolver));
        }

        [Test]
        public void JsonApiConfiguration_Validate_NoResourceConfiguration_ShouldThrowArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new JsonApiConfiguration().Validate());
            Assert.That(ex.ParamName, Is.EqualTo("ResourceConfigurations"));
        }

        [Test]
        public void JsonApiConfiguration_Validate_NoResourceConfiguration_DoesNotThrow()
        {
            var config = new JsonApiConfiguration
            {
                ResourceConfigurations = new ResourceConfigurationsBuilder().Build()
            };

            Assert.DoesNotThrow(() => config.Validate());
        }
    }
}
