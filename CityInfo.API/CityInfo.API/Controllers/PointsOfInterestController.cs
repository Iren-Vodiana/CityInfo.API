using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController:Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService; 
            //HttpContext.RequestServices.GetService
        }


        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                //throw new Exception();
                var city = CityDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found.");
                    return NotFound();
                }
                else
                {
                    return Ok(city.PointsOfInterest);
                }
            }
            catch(Exception e)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}. {e.Message}");
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{cityId}/pointsofinterest/{id}")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var pointOfInterest = CityDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId)?.PointsOfInterest.FirstOrDefault(x => x.Id == id);
            if (pointOfInterest == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(pointOfInterest);
            }
        }

        [HttpPost("{cityId}/pointsofinterest", Name = "GetPointOfInterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if(pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            //Look for fluent validation

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CityDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if(city == null)
            {
                return NotFound();
            }

            int maxPointOfInterestId = CityDataStore.Current.Cities.SelectMany(x => x.PointsOfInterest).Max(x => x.Id);

            var finalPointOfInterest = new Models.PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);
            
            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            //Look for fluent validation

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CityDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);

            if(pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, [FromBody]JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if(patchDoc == null)
            {
                return BadRequest();
            }

            var city = CityDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState); //cannot pass ModelState if TModel is not provided

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name.");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CityDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);

            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestFromStore);

            _mailService.Send("Point of interest was deleted.", $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

            return NoContent();
        }
    }
}
