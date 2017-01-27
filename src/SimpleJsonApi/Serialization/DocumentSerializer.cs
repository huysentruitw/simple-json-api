using System;
using System.Web.Http;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Serialization
{
    internal sealed class DocumentSerializer : IDocumentSerializer
    {
        public Document Serialize(object instance, Type type, JsonApiConfiguration configuration)
        {
            if (instance is HttpError) return SerializeHttpError((HttpError) instance, configuration);

            throw new NotImplementedException();
        }

        private Document SerializeHttpError(HttpError httpError, JsonApiConfiguration configuration)
        {
            return new Document
            {
                Errors = new []
                {
                    new Error
                    {
                        Title = httpError.Message,
                        Detail = httpError.ExceptionMessage + Environment.NewLine + httpError.StackTrace
                    }
                }
            };
        }
    }
}
