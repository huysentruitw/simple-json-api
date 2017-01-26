using System;
using Newtonsoft.Json;

namespace SimpleJsonApi.Exceptions
{
    public class JsonApiFormatException : JsonException
    {
        internal JsonApiFormatException(string message)
            : base($"Invalid JSON API Format: {message}")
        { }

        internal JsonApiFormatException(string message, Exception innerException)
            : base($"Invalid JSON API Format: {message}", innerException)
        { }
    }
}
