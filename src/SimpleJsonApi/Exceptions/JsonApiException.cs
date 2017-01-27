using System;
using Newtonsoft.Json;

namespace SimpleJsonApi.Exceptions
{
    internal class JsonApiException : JsonException
    {
        public JsonApiException(CausedBy causedBy, string message)
            : base(message)
        {
            CausedBy = causedBy;
        }

        public JsonApiException(CausedBy kind, string message, Exception innerException)
            : base(message, innerException)
        {
            CausedBy = kind;
        }

        public CausedBy CausedBy { get; }
    }
}
