using SimpleJsonApi.Configuration;
using SimpleJsonApi.Http;
using SimpleJsonApi.Serialization;

namespace System.Web.Http
{
    public static class HttpConfigurationExtensions
    {
        public static HttpConfiguration UseJsonApi(this HttpConfiguration httpConfiguration, Action<JsonApiConfiguration> configurationAction)
        {
            if (httpConfiguration == null) throw new ArgumentNullException(nameof(httpConfiguration));

            var jsonApiConfiguration = new JsonApiConfiguration();
            configurationAction?.Invoke(jsonApiConfiguration);
            jsonApiConfiguration.Validate();

            httpConfiguration.MessageHandlers.Add(new JsonApiDelegatingHandler(jsonApiConfiguration));
            httpConfiguration.Formatters.Add(new JsonApiMediaTypeFormatter(jsonApiConfiguration,
                () => new DocumentDeserializer(),
                () => new DocumentSerializer()));

            return httpConfiguration;
        }
    }
}
