using System.Web.Http;
using Microsoft.Owin;
using Newtonsoft.Json.Serialization;
using Owin;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.Sample.Models;

[assembly: OwinStartup(typeof(SimpleJsonApi.Sample.Startup))]

namespace SimpleJsonApi.Sample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var resourceConfig = new ResourceConfigurationsBuilder();
            resourceConfig
                .Resource<Car>();

            resourceConfig
                .Resource<Driver>()
                .BelongsTo<Car>(x => x.CarId);

            config.UseJsonApi(o =>
            {
                o.ResourceConfigurations = resourceConfig.Build();
            });

            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }
    }
}
