using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.DocumentConverters;
using SimpleJsonApi.Extensions;
using SimpleJsonApi.Http;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Tests.Http
{
    [TestFixture]
    public class JsonApiMediaTypeFormatterTests
    {
        private JsonApiConfiguration _config;
        private JsonSerializer _serializer;
        private Mock<IDocumentParser> _parserMock;
        private Mock<IDocumentBuilder> _builderMock;
        private Func<IDocumentParser> _parserFunc;
        private Func<IDocumentBuilder> _builderFunc;

        [SetUp]
        public void SetUp()
        {
            var builder = new ResourceConfigurationsBuilder();
            builder.Resource<TestResource>();

            _config = new JsonApiConfiguration { ResourceConfigurations = builder.Build() };

            _serializer = JsonSerializer.Create(_config.SerializerSettings);

            _parserMock = new Mock<IDocumentParser>();
            _parserMock
                .Setup(x => x.ParseDocument(It.IsAny<Document>(), typeof(TestResource)))
                .Returns<Document, Type>((d, t) => d);
            _parserFunc = () => _parserMock.Object;


            _builderMock = new Mock<IDocumentBuilder>();
            _builderFunc = () => _builderMock.Object;
        }

        [Test]
        public void JsonApiMediaTypeFormatter_Constructor_ShouldSetUTF8Encoding()
        {
            var formatter = new JsonApiMediaTypeFormatter(_config, _parserFunc, _builderFunc);
            Assert.That(formatter.SupportedEncodings.Count, Is.EqualTo(1));
            Assert.True(formatter.SupportedEncodings.Any(x => x is UTF8Encoding));
        }

        [Test]
        public void JsonApiMediaTypeFormatter_Constructor_ShouldSetMediaType()
        {
            var formatter = new JsonApiMediaTypeFormatter(_config, _parserFunc, _builderFunc);
            Assert.That(formatter.SupportedMediaTypes.Count, Is.EqualTo(1));
            Assert.True(formatter.SupportedMediaTypes.Any(x => x.MediaType == "application/vnd.api+json"));
        }

        [Test]
        public void JsonApiMediaTypeFormatter_CanReadType_ShouldReturnTrueForKnownResourceTypes()
        {
            var formatter = new JsonApiMediaTypeFormatter(_config, _parserFunc, _builderFunc);
            Assert.True(formatter.CanReadType(typeof(TestResource)));
            Assert.False(formatter.CanReadType(typeof(UnknownResource)));
            Assert.True(formatter.CanReadType(typeof(Changes<TestResource>)));
            Assert.False(formatter.CanReadType(typeof(Changes<UnknownResource>)));
            Assert.False(formatter.CanReadType(typeof(IEnumerable<UnknownResource>)));
        }

        [Test]
        public void JsonApiMediaTypeFormatter_CanWriteType_ShouldReturnTrueForKnownResourceTypes()
        {
            var formatter = new JsonApiMediaTypeFormatter(_config, _parserFunc, _builderFunc);
            Assert.True(formatter.CanWriteType(typeof(TestResource)));
            Assert.False(formatter.CanWriteType(typeof(UnknownResource)));
            Assert.True(formatter.CanWriteType(typeof(IEnumerable<TestResource>)));
            Assert.False(formatter.CanWriteType(typeof(IEnumerable<UnknownResource>)));
            Assert.False(formatter.CanWriteType(typeof(Changes<TestResource>)));
        }

        [Test]
        public void JsonApiMediaTypeFormatter_ReadFromStream_ShouldCallDocumentParser()
        {
            var formatter = new JsonApiMediaTypeFormatter(_config, _parserFunc, _builderFunc);
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
                {
                    writer.Write(@"{
                        data: {
                            type: 'myResource'
                        }
                    }");
                }

                stream.Seek(0, SeekOrigin.Begin);
                formatter.ReadFromStream(typeof(TestResource), stream, null, null);
            }

            _parserMock.Verify(x => x.ParseDocument(
                It.Is<Document>(y =>  y.Data.ParseAs<DocumentData>(_serializer).Type == "myResource"),
                typeof(TestResource)), Times.Once);
        }

        [Test]
        public void JsonApiMediaTypeFormatter_WriteToStream_ShouldCallDocumentBuilder()
        {
            var requestUri = new Uri("https://localhost/api/test");
            var formatter = new JsonApiMediaTypeFormatter(_config, _parserFunc, _builderFunc)
                .GetPerRequestFormatterInstance(
                    typeof(TestResource),
                    new HttpRequestMessage(HttpMethod.Get, requestUri),
                    new MediaTypeHeaderValue("application/vnd.api+json"));

            var resource = new TestResource();
            var stream = new MemoryStream();

            formatter.WriteToStreamAsync(typeof(TestResource), resource, stream, null, null).Wait();

            _builderMock.Verify(x => x.BuildDocument(resource, typeof(TestResource), requestUri), Times.Once);
        }

        private class TestResource
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        private class UnknownResource
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}
