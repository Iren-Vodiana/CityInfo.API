using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        //public string Description { get; set; }

        [MaxLength(200)]
        public String Description { get; set; }

        [ForeignKey("CityId")]
        public int CityId { get; set; }
        public City City { get; set; } //navigation property
    }
}
