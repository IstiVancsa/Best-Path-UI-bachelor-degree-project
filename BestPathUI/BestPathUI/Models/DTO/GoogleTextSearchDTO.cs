using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DTO
{
    public class GoogleTextSearchDTO
    {
        public string formatted_address { get; set; }
        public GeometryDTO geometry { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public OpeningHourDTO opening_hours { get; set; } = new OpeningHourDTO();
        public string place_id { get; set; }
        public float price_level { get; set; }
        public float rating { get; set; }
        public int user_ratings_total { get; set; }
    }
}
