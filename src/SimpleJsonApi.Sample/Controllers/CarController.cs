using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using SimpleJsonApi.Sample.Models;

namespace SimpleJsonApi.Sample.Controllers
{
    [RoutePrefix("api/cars")]
    public class CarController : ApiController
    {
        private static readonly Dictionary<Guid, Car> Cars = new Dictionary<Guid, Car>();

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<Car>))]
        public IHttpActionResult GetCars()
        {
            return Ok(Cars.Select(x => x.Value));
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(Car))]
        public IHttpActionResult GetCar(Guid id)
        {
            if (!Cars.ContainsKey(id)) return NotFound();
            return Ok(Cars[id]);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateCar(Car car)
        {
            car.Id = Guid.NewGuid();
            Cars.Add(car.Id, car);
            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateCar(Guid id, Car car)
        {
            if (!Cars.ContainsKey(id)) return NotFound();
            Cars[id] = car;
            return Ok();
        }

        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult PartiallyUpdateCar(Guid id, Changes<Car> carChanges)
        {
            if (!Cars.ContainsKey(id)) return NotFound();
            carChanges.ApplyTo(Cars[id]);
            return Ok();
        }
    }
}