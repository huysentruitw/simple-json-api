using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Exceptions;
using SimpleJsonApi.Models;
using SimpleJsonApi.Serialization;

namespace SimpleJsonApi.Formatters
{
    internal sealed class JsonApiMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        private readonly JsonApiConfiguration _configuration;
        private readonly Func<IDocumentDeserializer> _documentDeserializerFactory;
        private readonly Func<IDocumentSerializer> _documentSerializerFactory;
        private readonly JsonSerializer _jsonSerializer;

        public JsonApiMediaTypeFormatter(JsonApiConfiguration configuration,
            Func<IDocumentDeserializer> documentDeserializerFactory,
            Func<IDocumentSerializer> documentSerializerFactory)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (documentDeserializerFactory == null) throw new ArgumentNullException(nameof(documentDeserializerFactory));
            _configuration = configuration;
            _documentDeserializerFactory = documentDeserializerFactory;
            _documentSerializerFactory = documentSerializerFactory;
            _jsonSerializer = JsonSerializer.Create(configuration.SerializerSettings);

            SupportedEncodings.Add(new UTF8Encoding(false, true));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(_configuration.MediaType));
        }

        public override bool CanReadType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Changes<>))
                type = type.GetGenericArguments().First();
            return _configuration.ResourceConfiguration.IsMapped(type);
        }

        public override bool CanWriteType(Type type)
        {
            return _configuration.ResourceConfiguration.IsMapped(type);
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            using (var streamReader = new StreamReader(readStream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var document = _jsonSerializer.Deserialize<UpdateDocument>(jsonTextReader);
                if (document?.Data == null) throw new JsonApiFormatException("data is missing");
                if (string.IsNullOrEmpty(document.Data.Type)) throw new JsonApiFormatException("type is missing");
                return _documentDeserializerFactory().Deserialize(document, type, _configuration);
            }
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var streamWriter = new StreamWriter(writeStream))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                var document = _documentSerializerFactory().Serialize(value, type, _configuration);
                _jsonSerializer.Serialize(jsonTextWriter, document);
            }
        }
    }
}
