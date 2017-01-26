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
        private readonly Dictionary<Guid, Driver> _drivers = new Dictionary<Guid, Driver>();

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<Driver>))]
        public IHttpActionResult GetDrivers()
        {
            return Ok(_drivers);
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(Driver))]
        public IHttpActionResult GetDriver(Guid id)
        {
            if (!_drivers.ContainsKey(id)) return NotFound();
            return Ok(_drivers[id]);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateDriver(Driver driver)
        {
            driver.Id = Guid.NewGuid();
            _drivers.Add(driver.Id, driver);
            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateDriver(Guid id, Driver driver)
        {
            if (!_drivers.ContainsKey(id)) return NotFound();
            _drivers[id] = driver;
            return Ok();
        }

        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult PartiallyUpdateDriver(Guid id, Changes<Driver> driverChanges)
        {
            if (!_drivers.ContainsKey(id)) return NotFound();
            driverChanges.ApplyTo(_drivers[id]);
            return Ok();
        }
    }
}
