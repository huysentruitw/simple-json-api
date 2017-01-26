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
using SimpleJsonApi.Extensions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Formatters
{
    internal sealed class JsonApiMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        public const string JsonApiMediaType = "application/vnd.api+json";
        private readonly JsonApiConfiguration _configuration;
        private readonly JsonSerializer _jsonSerializer;

        public JsonApiMediaTypeFormatter(JsonApiConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
            _jsonSerializer = JsonSerializer.Create(configuration.SerializerSettings);

            SupportedEncodings.Add(new UTF8Encoding(false, true));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(JsonApiMediaType));
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
                var document = _jsonSerializer.Deserialize<Document>(jsonTextReader);
                if (document?.Data == null) throw new JsonApiFormatException("data is missing");
                if (string.IsNullOrEmpty(document.Data.Type)) throw new JsonApiFormatException("type is missing");
                return document.ConvertTo(type, _configuration);
            }
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            base.WriteToStream(type, value, writeStream, content);
        }
    }
}
