using Models.DTO;
using Models.Extension;
using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class City : BaseModel
    {
        [Required]
        [Display(Name = "CityName")]
        public string CityName { get; set; }
        [Unlike("StartPoint")]
        public bool DestinationPoint { get; set; }

        [Unlike("DestinationPoint")]
        public bool StartPoint { get; set; }
        public bool NeedsRestaurant { get; set; }
        public string RestaurantType { get; set; }
        public bool NeedsMuseum { get; set; }
        public string MuseumType { get; set; }
        public LocationDTO Location { get; set; }
        public GoogleTextSearchDTO SelectedRestaurant { get; set; }
        public GoogleTextSearchDTO SelectedMuseum { get; set; }
        public string UserId { get; set; }

        public new CityDTO GetDTO()
        {
            return new CityDTO
            {
                Id = this.Id,
                CityName = this.CityName,
                DestinationPoint = this.DestinationPoint,
                MuseumType = this.MuseumType,
                NeedsMuseum = this.NeedsMuseum,
                NeedsRestaurant = this.NeedsRestaurant,
                RestaurantType = this.RestaurantType,
                StartPoint = this.StartPoint,
                Location = this.Location,
                UserId = this.UserId
            };
        }
        public City()
        {
            Id = Guid.NewGuid();
            CityName = "";
            DestinationPoint = false;
            MuseumType = "";
            NeedsMuseum = false;
            NeedsRestaurant = false;
            RestaurantType = "";
            StartPoint = false;
            Location = new LocationDTO();
            SelectedMuseum = new GoogleTextSearchDTO();
            SelectedRestaurant = new GoogleTextSearchDTO();
            UserId = "";
        }
    }
}
