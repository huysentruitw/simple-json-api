using Newtonsoft.Json;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.DocumentConverters;
using SimpleJsonApi.Http;

namespace System.Web.Http
{
    /// <summary>
    /// Extensions methods for <see cref="HttpConfiguration"/>.
    /// </summary>
    public static class HttpConfigurationExtensions
    {
        /// <summary>
        /// Configures JSON API message handler and media type formatter on the given <see cref="HttpConfiguration"/> instance.
        /// </summary>
        /// <param name="httpConfiguration">The <see cref="HttpConfiguration"/> instance.</param>
        /// <param name="configurationAction">An action to modify the default <see cref="JsonApiConfiguration"/>.</param>
        /// <returns>The <see cref="HttpConfiguration"/> instance for method chaining purposes.</returns>
        public static HttpConfiguration UseJsonApi(this HttpConfiguration httpConfiguration, Action<JsonApiConfiguration> configurationAction)
        {
            if (httpConfiguration == null) throw new ArgumentNullException(nameof(httpConfiguration));
            if (configurationAction == null) throw new ArgumentNullException(nameof(configurationAction));

            var jsonApiConfiguration = new JsonApiConfiguration();
            configurationAction(jsonApiConfiguration);
            jsonApiConfiguration.Validate();

            httpConfiguration.MessageHandlers.Add(new JsonApiDelegatingHandler(jsonApiConfiguration));

            var jsonSerializer = JsonSerializer.Create(jsonApiConfiguration.SerializerSettings);

            httpConfiguration.Formatters.Add(new JsonApiMediaTypeFormatter(jsonApiConfiguration,
                () => new DocumentParser(jsonApiConfiguration.ResourceConfigurations, jsonSerializer),
                () => new DocumentBuilder(jsonApiConfiguration.ResourceConfigurations)));

            return httpConfiguration;
        }
    }
}
