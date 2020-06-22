using BestPathUI.Persistence;
using Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models.DTO;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPathUI.Pages.Components
{
    public class AddCityDialogBase : ComponentBase
    {
        [Inject]
        public IGoogleDataService GoogleDataService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }//this is used so we can call js methods inside our cs files
        public City City { get; set; } = new City();
        public List<string> RestaurantTypes { get; set; }
        public List<string> MuseumTypes { get; set; }
        [Parameter]//now we can cann it as parameter in razor file
        public EventCallback<Map_AddCity> CloseEventCallBack { get; set; }//we are sending a message from addcitydialog to maps overview

        public bool ShowDialog { get; set; }
        public static LocationDTO Location { get; set; }
        public IList<GoogleTextSearchDTO> RestaurantSearches { get; set; } = new List<GoogleTextSearchDTO>();
        public IList<GoogleTextSearchDTO> MuseumSearches { get; set; } = new List<GoogleTextSearchDTO>();
        private bool _autocompleteInitialized { get; set; } = false;
        public string RestaurantType { get; set; }
        public string MuseumType { get; set; }
        public bool AStarCity { get; set; }
        private string _hourSelected { get; set; }
        private string _minuteSelected { get; set; }
        public string TimeSelectTitle { get; set; } = "Arriving Time: ";

        protected override void OnInitialized()
        {
            RestaurantTypes = new List<string>
            {
                "Mexican",
                "Italian",
                "Romanian",
                "France",
                "American",
                "Chinese",
                "All"
            };

            MuseumTypes = new List<string>
            {
                "General",
                "Natural History",
                "Natural Science",
                "History",
                "Art",
                "Virtual",
                "All"
            };
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (ShowDialog & !_autocompleteInitialized)
            {
                _autocompleteInitialized = true;
                await JSRuntime.InvokeVoidAsync("initializeAutocompletes");
            }
        }
        public void Show(bool aStarCity = false)
        {
            ResetDialog();
            AStarCity = aStarCity;
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            _autocompleteInitialized = false;
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            this.City = new City
            {
                CityName = "",
                DestinationPoint = false,
                MuseumType = "",
                NeedsMuseum = false,
                NeedsRestaurant = false,
                RestaurantType = "",
                StartPoint = false,
                ArrivingTime = DateTime.Now
            };
            TimeSelectTitle = "Arriving Time: ";
        }

        protected void SetTimeSelectTitle()
        {
            if (!City.StartPoint)
                this.TimeSelectTitle = "Route Start Time: ";
            else
                this.TimeSelectTitle = "Arriving Time: ";
            StateHasChanged();
        }

        protected void SaveDate()
        {
            this.City.ArrivingTime = new DateTime(
                this.City.ArrivingTime.Year,
                this.City.ArrivingTime.Month,
                this.City.ArrivingTime.Day,
                Convert.ToInt32(this._hourSelected),
                Convert.ToInt32(this._minuteSelected),
                0,
                0,
                this.City.ArrivingTime.Kind
                );
        }

        protected void RestaurantClicked(ChangeEventArgs restaurantEvent)
        {
            if (string.Equals(restaurantEvent.Value.ToString(), "All"))
                this.RestaurantType = "";
            else
                this.RestaurantType = "+" + restaurantEvent.Value.ToString();
        }

        protected void MuseumClicked(ChangeEventArgs museumEvent)
        {
            if (string.Equals(museumEvent.Value.ToString(), "All"))
                this.MuseumType = "";
            else
                this.MuseumType = "+" + museumEvent.Value.ToString();
        }

        protected void HourClicked(ChangeEventArgs hourEvent)
        {
            _hourSelected = hourEvent.Value.ToString();
        }

        protected void MinuteClicked(ChangeEventArgs minuteEvent)
        {
            _minuteSelected = minuteEvent.Value.ToString();
        }

        protected async Task HandleValidSubmit()
        {
            this.City.Location = Location;

            Map_AddCity map_AddCity = new Map_AddCity();
            if (this.City.NeedsMuseum)
            {
                map_AddCity.MuseumSearches = (await GoogleDataService.TextSearch(MuseumType + "Museum", this.City.Location)).results;
            }
            if (this.City.NeedsRestaurant)
            {
                map_AddCity.RestaurantSearches = (await GoogleDataService.TextSearch(RestaurantType + "Restaurant", this.City.Location)).results;
            }

            map_AddCity.City = this.City;

            await CloseEventCallBack.InvokeAsync(map_AddCity);//we can send even the save employee here
            ShowDialog = false;
            _autocompleteInitialized = false;
            StateHasChanged();
        }
        [JSInvokable]
        public static void SetLocation(LocationDTO location)
        {
            Location = location;
        }
    }
}
