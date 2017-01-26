using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using SimpleJsonApi.Sample.Models;

namespace SimpleJsonApi.Sample.Controllers
{
    [RoutePrefix("api/cars")]
    public class CarController : ApiController
    {
        private readonly Dictionary<Guid, Car> _cars = new Dictionary<Guid, Car>();

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<Car>))]
        public IHttpActionResult GetCars()
        {
            return Ok(_cars);
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(Car))]
        public IHttpActionResult GetCar(Guid id)
        {
            if (!_cars.ContainsKey(id)) return NotFound();
            return Ok(_cars[id]);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateCar(Car car)
        {
            car.Id = Guid.NewGuid();
            _cars.Add(car.Id, car);
            return Ok();
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateCar(Guid id, Car car)
        {
            if (!_cars.ContainsKey(id)) return NotFound();
            _cars[id] = car;
            return Ok();
        }

        [HttpPatch]
        [Route("{id}")]
        public IHttpActionResult PartiallyUpdateCar(Guid id, Changes<Car> carChanges)
        {
            if (!_cars.ContainsKey(id)) return NotFound();
            carChanges.ApplyTo(_cars[id]);
            return Ok();
        }
    }
}