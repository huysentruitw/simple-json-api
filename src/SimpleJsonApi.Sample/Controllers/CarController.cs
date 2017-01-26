using System.Web.Http;
using SimpleJsonApi.Sample.Models;

namespace SimpleJsonApi.Sample.Controllers
{
    [RoutePrefix("api/cars")]
    public class CarController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateCar(Car car)
        {
            return Ok();
        }
    }
}