using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public class CityDataStore
    {
        public static CityDataStore Current { get; } = new CityDataStore();
        
        public List<CityDto> Cities { get; set; }

        public CityDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "New York",
                    Description = "Cool city",
                    PointsOfInterest = new List<PointOfInterestDto>() {
                        new PointOfInterestDto() {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited place" }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Warsaw",
                    Description = "My city",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Culture Palace",
                            Description = "Centrum of the city"
                        }
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Kijev",
                    Description = "The capital of Ukraine"
                }
            };
        }
    }
}
