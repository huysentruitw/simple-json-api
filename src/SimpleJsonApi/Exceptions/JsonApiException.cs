using System;
using Newtonsoft.Json;

namespace SimpleJsonApi.Exceptions
{
    public class JsonApiException : JsonException
    {
        public JsonApiException(string message)
            : base(message)
        { }

        public JsonApiException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
