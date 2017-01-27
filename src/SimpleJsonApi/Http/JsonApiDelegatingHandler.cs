using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

            if (!request.Headers.Accept.Any(x => x.MediaType.Equals(_configuration.MediaType)) ||
                ((int) result.StatusCode >= 400 && (int) result.StatusCode < 500))
                return result;

            return result;
        }
    }
}
