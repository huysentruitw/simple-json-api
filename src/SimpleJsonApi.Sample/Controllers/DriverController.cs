using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using SimpleJsonApi.Sample.Models;

namespace SimpleJsonApi.Sample.Controllers
{
    [RoutePrefix("api/drivers")]
    public class DriverController : ApiController
    {
        private static readonly Dictionary<Guid, Driver> Drivers = new Dictionary<Guid, Driver>();

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<Driver>))]
        public IHttpActionResult GetDrivers()
        {
            return Ok(Drivers.Values);
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
}
