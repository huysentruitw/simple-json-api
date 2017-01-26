using System;
using System.Web.Http;
using SimpleJsonApi.Sample.Models;

namespace SimpleJsonApi.Sample.Controllers
{
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
}
