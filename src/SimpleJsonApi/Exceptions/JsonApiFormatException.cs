using System;

namespace SimpleJsonApi.Exceptions
{
    internal sealed class JsonApiFormatException : JsonApiException
    {
        internal JsonApiFormatException(CausedBy causedBy, string message)
            : base(causedBy, $"Invalid JSON API Format: {message}")
        { }

        internal JsonApiFormatException(CausedBy causedBy, string message, Exception innerException)
            : base(causedBy, $"Invalid JSON API Format: {message}", innerException)
        { }
    }
}
