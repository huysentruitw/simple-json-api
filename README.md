# SimpleJsonApi

[![Build status](https://ci.appveyor.com/api/projects/status/4q1chjxv30acjbt8/branch/master?svg=true)](https://ci.appveyor.com/project/huysentruitw/simple-json-api/branch/master)

## Description

Simple JSON API server implementation for ASP.NET Web API 2. Based on the [JSON API 1.0 specification](http://jsonapi.org/format/).

* Converts incoming JSON API documents into a model (for POST/PUT operations)
* Applies an incoming JSON API document onto an existing model (for PATCH operations)
* Incoming models support attributes and relationships
* Outgoing models currently support a self link, attributes and relationships (includes and meta not implemented yet)
* Translates exceptions into JSON API error documents

For requesting one or more resources, the GET request must include an Accept header with the value 'application/vnd.api+json'.
For posting/putting/patching/deleting resource, the POST/PUT/PATCH/DELETE request must include a Content-Type header with the value 'application/vnd.api+json'.

## Get it on NuGet

    Install-Package SimpleJsonApi

## Usage example

### Model definitions

```C#
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
```

### Configuration

```C#
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
```

### Controller

```C#
[RoutePrefix("api/drivers")]
public class DriverController : ApiController
{
    private static readonly Dictionary<Guid, Driver> Drivers = new Dictionary<Guid, Driver>();

    [HttpGet]
    [Route("")]
    [ResponseType(typeof(IEnumerable<Driver>))]
    public IHttpActionResult GetDrivers()
    {
        return Ok(Drivers.Select(x => x.Value));
    }

    [HttpGet]
    [Route("{id}")]
    [ResponseType(typeof(Driver))]
    public IHttpActionResult GetDriver(Guid id)
    {
        if (!Drivers.ContainsKey(id)) return NotFound();
        return Ok(Drivers[id]);
    }

    [HttpPost]
    [Route("")]
    public IHttpActionResult CreateDriver(Driver driver)
    {
        driver.Id = Guid.NewGuid();
        Drivers.Add(driver.Id, driver);
        return Ok();
    }

    [HttpPut]
    [Route("{id}")]
    public IHttpActionResult UpdateDriver(Guid id, Driver driver)
    {
        if (!Drivers.ContainsKey(id)) return NotFound();
        Drivers[id] = driver;
        return Ok();
    }

    [HttpPatch]
    [Route("{id}")]
    public IHttpActionResult PartiallyUpdateDriver(Guid id, Changes<Driver> driverChanges)
    {
        if (!Drivers.ContainsKey(id)) return NotFound();
        driverChanges.ApplyTo(Drivers[id]);
        return Ok();
    }
}
```

### Adding a driver

Send a POST request with this raw JSON data with Content-Type header 'application/vnd.api+json' to '/api/drivers'.

```json
{
	"data": {
		"type": "drivers",
		"attributes": {
			"name": "John Doe",
			"licensed": true
		},
		"relationships": {
			"car": {
				"data": {
					"type": "cars",
					"id": "8bf9f9f8-12e9-4b00-8a99-bea1e1701ce5"
				}
			}
		}
	}
}
```

### Requesting all drivers

Send a GET request with Accept header 'application/vnd.api+json' to '/api/drivers' to get the following output:

```json
{
	"links": {
		"self": "http://localhost:56373/api/drivers"
	},
	"data": [
		{
			"id": "d385f819-237c-4cdf-a198-76b17d5a4a01",
			"type": "drivers",
			"attributes": {
				"name": "John Doe",
				"licensed": true
			},
			"relationships": {
				"car": {
					"data": {
						"type": "cars",
						"id": "8bf9f9f8-12e9-4b00-8a99-bea1e1701ce5"
					}
				}
			}
		}
	]
}
```
