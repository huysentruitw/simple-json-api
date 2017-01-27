using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Exceptions;
using SimpleJsonApi.Models;
using SimpleJsonApi.Serialization;

namespace SimpleJsonApi.Http
{
    internal sealed class JsonApiMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        private readonly JsonApiConfiguration _configuration;
        private readonly Func<IDocumentDeserializer> _documentDeserializerFunc;
        private readonly Func<IDocumentSerializer> _documentSerializerFunc;
        private readonly JsonSerializer _jsonSerializer;

        public JsonApiMediaTypeFormatter(JsonApiConfiguration configuration,
            Func<IDocumentDeserializer> documentDeserializerFunc,
            Func<IDocumentSerializer> documentSerializerFunc)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (documentDeserializerFunc == null) throw new ArgumentNullException(nameof(documentDeserializerFunc));
            if (documentSerializerFunc == null) throw new ArgumentNullException(nameof(documentSerializerFunc));
            _configuration = configuration;
            _documentDeserializerFunc = documentDeserializerFunc;
            _documentSerializerFunc = documentSerializerFunc;
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
            return _configuration.ResourceConfiguration.IsMapped(type) || (type == typeof(HttpError));
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            using (var streamReader = new StreamReader(readStream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var document = _jsonSerializer.Deserialize<UpdateDocument>(jsonTextReader);
                if (document?.Data == null) throw new JsonApiFormatException(CausedBy.Client, "data is missing");
                if (string.IsNullOrEmpty(document.Data.Type)) throw new JsonApiFormatException(CausedBy.Client, "type is missing");
                return _documentDeserializerFunc().Deserialize(document, type, _configuration);
            }
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var streamWriter = new StreamWriter(writeStream))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                var document = _documentSerializerFunc().Serialize(value, type, _configuration);
                _jsonSerializer.Serialize(jsonTextWriter, document);
            }
        }
    }
}
