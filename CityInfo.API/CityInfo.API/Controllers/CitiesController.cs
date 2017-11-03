using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    public class CitiesController : Controller
    {
        //[HttpGet("api/cities")]
        [HttpGet()]
        public JsonResult GetCities()
        {
            //var object1 = new { id = 3, Name = "Kijev" };
            //return new JsonResult(new List<object>()
            //{
            //    new { id = 1, Name = "New York City"},
            //    new { id = 2, Name = "Warsaw"},
            //    object1
            //});
            return new JsonResult(CityDataStore.Current.Cities) { StatusCode = 200 };
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            var cityToFind = CityDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
            if(cityToFind == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(cityToFind);
            }
        }
    }
}
