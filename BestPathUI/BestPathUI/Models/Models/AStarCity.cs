using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Models
{
    public class AStarCity
    {
        public City City { get; set; }
        public bool Visited { get; set; }
        public bool IsOrigin { get; set; }
        public bool IsDestination { get; set; }
        public AStarCity(City city)
        {
            this.City = city;
            if (city.StartPoint)
                this.IsOrigin = true;
            if (city.DestinationPoint)
                this.IsDestination = true;
        }
    }
}
