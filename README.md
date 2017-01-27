# SimpleJsonApi

## Description

Simple JSON API server implementation for ASP.NET Web API 2. Based on the [JSON API 1.0 specification](http://jsonapi.org/format/).

* Converts incoming JSON API documents into a model (for POST/PUT operations)
* Applies an incoming JSON API document onto an existing model (for PATCH operations)

## Usage

    public class Car
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
    }
    
    public class Driver
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Licensed { get; set; }
        [JsonProperty("car")]
        public Guid CarId { get; set; }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var resourceConfig = new ResourceConfigurationBuilder();
            resourceConfig
                .Resource<Car>()
                .MapAllProperties();

            resourceConfig
                .Resource<Driver>()
                .MapAllProperties()
                .BelongsTo<Car>(x => x.CarId);

            config.UseJsonApi(o =>
            {
                o.ResourceConfiguration = resourceConfig.Build();
            });

            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }
    }

    [RoutePrefix("api/drivers")]
    public class DriverController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateDriver(Driver driver)
        {
            return Ok();
        }

        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult UpdateDriver(Guid id, Changes<Driver> driverChanges)
        {
            var driver = new Driver();
            driverChanges.ApplyTo(driver);
            return Ok();
        }
    }
