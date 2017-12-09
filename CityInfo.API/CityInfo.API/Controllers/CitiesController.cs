using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
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
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        //[HttpGet("api/cities")]
        [HttpGet()]
        public IActionResult GetCities()
        {
            //var object1 = new { id = 3, Name = "Kijev" };
            //return new JsonResult(new List<object>()
            //{
            //    new { id = 1, Name = "New York City"},
            //    new { id = 2, Name = "Warsaw"},
            //    object1
            //});

            //return new JsonResult(CityDataStore.Current.Cities) { StatusCode = 200 };
             
            var cityEntities = _cityInfoRepository.GetCities();
            var results = Mapper.Map < IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            //var cityToFind = CityDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
            var cityToFind = _cityInfoRepository.GetCity(id, includePointsOfInterest);
            if(cityToFind == null)
            {
                return NotFound();
            }
            else
            {
                if(includePointsOfInterest)
                {
                    var cityResult = Mapper.Map<CityDto>(cityToFind);
                    //var cityResult = new CityDto()
                    //{
                    //    Id = cityToFind.Id,
                    //    Name = cityToFind.Name,
                    //    Description = cityToFind.Description                        
                    //};

                    //foreach(var poi in cityToFind.PointsOfInterest)
                    //{
                    //    cityResult.PointsOfInterest.Add(new PointOfInterestDto() {
                    //        Id = poi.Id,
                    //        Name = poi.Name,
                    //        Description = poi.Description
                    //    });
                    //}

                    return Ok(cityResult);
                }

                //var cityWithoutPointsOfInterestResult = new CityWithoutPointsOfInterestDto()
                //{
                //    Id = cityToFind.Id,
                //    Name = cityToFind.Name,
                //    Description = cityToFind.Description
                //};                

                var cityWithoutPointsOfInterestResult = Mapper.Map<CityWithoutPointsOfInterestDto>(cityToFind);
                return Ok(cityWithoutPointsOfInterestResult);
            }
        }
    }
}
