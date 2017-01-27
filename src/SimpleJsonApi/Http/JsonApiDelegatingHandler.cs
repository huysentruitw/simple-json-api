using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using SimpleJsonApi.Configuration;

namespace SimpleJsonApi.Http
{
    internal class JsonApiDelegatingHandler : DelegatingHandler
    {
        private readonly JsonApiConfiguration _configuration;

        public JsonApiDelegatingHandler(JsonApiConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await base.SendAsync(request, cancellationToken);

            var jsonApiHeaderPresent = request.Headers.Accept.Any(x => x.MediaType.Equals(_configuration.MediaType));
            var statusCode = (int) result.StatusCode;
            if (!jsonApiHeaderPresent || (statusCode >= 400 && statusCode < 500))
                return result;

            var content = result.Content as ObjectContent;



            if (result.Content is ObjectContent<HttpError>)
            {
                
            }

            return result;
        }
    }
}
