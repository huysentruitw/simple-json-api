using SimpleJsonApi.Configuration;
using SimpleJsonApi.Formatters;

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

            // TODO httpConfiguration.MessageHandlers.Add(new JsonApiDelegatingHandler(jsonApiConfiguration));
            httpConfiguration.Formatters.Add(new JsonApiMediaTypeFormatter(jsonApiConfiguration));

            return httpConfiguration;
        }
    }
}
