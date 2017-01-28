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
using SimpleJsonApi.DocumentConverters;
using SimpleJsonApi.Exceptions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Http
{
    internal sealed class JsonApiMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        private readonly JsonApiConfiguration _configuration;
        private readonly Func<IDocumentParser> _documentParserFunc;
        private readonly Func<IDocumentBuilder> _documentBuilderFunc;
        private readonly JsonSerializer _jsonSerializer;
        private readonly HttpRequestMessage _request;

        public JsonApiMediaTypeFormatter(JsonApiConfiguration configuration,
            Func<IDocumentParser> documentParserFunc,
            Func<IDocumentBuilder> documentBuilderFunc)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (documentParserFunc == null) throw new ArgumentNullException(nameof(documentParserFunc));
            if (documentBuilderFunc == null) throw new ArgumentNullException(nameof(documentBuilderFunc));
            _configuration = configuration;
            _documentParserFunc = documentParserFunc;
            _documentBuilderFunc = documentBuilderFunc;
            _jsonSerializer = JsonSerializer.Create(configuration.SerializerSettings);

            SupportedEncodings.Add(new UTF8Encoding(false, true));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(_configuration.MediaType));
        }

        private JsonApiMediaTypeFormatter(HttpRequestMessage request, JsonApiConfiguration configuration,
            Func<IDocumentParser> documentParserFunc,
            Func<IDocumentBuilder> documentBuilderFunc)
            : this(configuration, documentParserFunc, documentBuilderFunc)
        {
            _request = request;
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new JsonApiMediaTypeFormatter(request, _configuration, _documentParserFunc, _documentBuilderFunc);
        }

        public override bool CanReadType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Changes<>))
                type = type.GetGenericArguments().First();
            return _configuration.ResourceConfigurations.Contains(type);
        }

        public override bool CanWriteType(Type type)
        {
            return _configuration.ResourceConfigurations.Contains(type) || (type == typeof(HttpError));
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            using (var streamReader = new StreamReader(readStream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var document = _jsonSerializer.Deserialize<Document>(jsonTextReader);
                if (document?.Data == null) throw new JsonApiFormatException(CausedBy.Client, "data is missing");
                if (string.IsNullOrEmpty(document.Data.Type)) throw new JsonApiFormatException(CausedBy.Client, "type is missing");
                return _documentParserFunc().ParseDocument(document, type);
            }
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var streamWriter = new StreamWriter(writeStream))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                var document = _documentBuilderFunc().BuildDocument(value, type, _request.RequestUri);
                _jsonSerializer.Serialize(jsonTextWriter, document);
            }
        }
    }
}
