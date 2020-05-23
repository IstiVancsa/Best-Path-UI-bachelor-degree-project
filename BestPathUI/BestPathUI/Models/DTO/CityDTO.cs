using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DTO
{
    public class CityDTO : BaseDTO
    {
        public string CityName { get; set; }
        public bool DestinationPoint { get; set; }
        public bool StartPoint { get; set; }
        public bool NeedsRestaurant { get; set; }
        public string RestaurantType { get; set; }
        public bool NeedsMuseum { get; set; }
        public string MuseumType { get; set; }
        public LocationDTO Location { get; set; }
        public string UserId { get; set; }
    }
}
