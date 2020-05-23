using Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Models
{
    public class Map_AddCity
    {
        public IList<GoogleTextSearchDTO> RestaurantSearches { get; set; } = new List<GoogleTextSearchDTO>();
        public IList<GoogleTextSearchDTO> MuseumSearches { get; set; } = new List<GoogleTextSearchDTO>();
        public City City { get; set; } = new City();
    }
}
