using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.DTO
{
    public class GetRoutesResultDTO : BaseTokenizedDTO
    {
        public List<Tuple<DateTime, List<City>>> Routes { get; set; }
        public GetRoutesResultDTO()
        {
            this.Routes = new List<Tuple<DateTime, List<City>>>();
        }
        public void SetLocation()
        {
            foreach(var route in Routes)
                foreach(var city in route.Item2)
                {
                    if (city.NeedsMuseum)
                        city.SelectedMuseum = new GoogleTextSearchDTO { geometry = new GeometryDTO { location = city.Location } };
                    if (city.NeedsRestaurant)
                        city.SelectedRestaurant = new GoogleTextSearchDTO { geometry = new GeometryDTO { location = city.Location } };
                }
        }
    }
}
