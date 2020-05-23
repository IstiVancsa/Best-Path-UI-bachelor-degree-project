using BestPathUI.Pages.Components;
using Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models.DTO;
using Models.Filters;
using Models.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace BestPathUI.Pages.MapPage
{
    public class MapBase : ComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }//this is used so we can call js methods inside our cs files
        [Inject]
        public ICitiesDataService CitiesDataService { get; set; }
        //[Inject]
        //public ISessionStorageDataService SessionStorage { get; set; }
        [Inject]
        public ILocalStorageManagerService LocalStorageManagerService { get; set; }
        public IList<City> Cities { get; set; }
        protected AddCityDialog AddCityDialog { get; set; }
        public IList<GoogleTextSearchDTO> RestaurantSearches { get; set; } = new List<GoogleTextSearchDTO>();
        public IList<GoogleTextSearchDTO> MuseumSearches { get; set; } = new List<GoogleTextSearchDTO>();
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public GetLastRouteResult LastRoutes { get; set; }
        public bool ShowSuccessAlert_Prop { get; set; }
        public bool ShowUnSuccessAlert_Prop { get; set; }
        public string SuccessAlertMessage { get; set; }
        public string UnSuccessAlertMessage { get; set; }
        public Timer SuccessAlertTimer { get; set; }
        public Timer UnSuccessAlertTimer { get; set; }
        protected override async Task OnInitializedAsync()
        {
            Cities = new List<City>();
            LastRoutes = new GetLastRouteResult();
            SuccessAlertTimer = new Timer(3000);
            SuccessAlertTimer.Elapsed += new ElapsedEventHandler((Object source, ElapsedEventArgs e) =>
            {
                InvokeAsync(() =>
                {
                    this.ShowSuccessAlert_Prop = false;
                    this.SuccessAlertTimer.Enabled = false;
                    StateHasChanged();
                });
            });

            UnSuccessAlertTimer = new Timer(3000);
            UnSuccessAlertTimer.Elapsed += new ElapsedEventHandler((Object source, ElapsedEventArgs e) =>
            {
                InvokeAsync(() =>
                {
                    this.ShowUnSuccessAlert_Prop = false;
                    this.UnSuccessAlertTimer.Enabled = false;
                    StateHasChanged();
                });
            });
        }
        private bool _mapInitialized { get; set; } = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_mapInitialized)
            {
                _mapInitialized = true;
                await JSRuntime.InvokeVoidAsync("createMap");
                await JSRuntime.InvokeVoidAsync("initializeMap");
            }
            var Token = await this.LocalStorageManagerService.GetPermanentItemAsync("Token");
            if (Token == null)
                NavigationManager.NavigateTo("/Login");
        }

        protected void AddCity()
        {
            AddCityDialog.Show();
        }

        protected async Task ShowRoute()
        {
            await JSRuntime.InvokeVoidAsync("removeDirections");
            var startPoint = GetStartPointGeoCoordinates();
            var endPoint = GetDestinationPointGeoCoordinates();
            var intermediatePoints = GetIntermediatePointsGeoCoordinates();
            if (startPoint != null && endPoint != null)
                await JSRuntime.InvokeVoidAsync("showRoute", startPoint, endPoint, intermediatePoints);
        }

        protected async Task NewRoute()
        {
            this.Cities.Clear();
            await JSRuntime.InvokeVoidAsync("removeDirections");
            await LocalStorageManagerService.DeletePermanentItemAsync("Cities");
            ShowSuccessAlert("Here we go! Now you can start all over again!");
            StateHasChanged();
        }

        protected async Task SaveRoute()
        {
            if (this.Cities.Count > 0)
            {
                await CitiesDataService.SavePathAsync(Cities);
                await LocalStorageManagerService.DeletePermanentItemAsync("Cities");
                ShowSuccessAlert("Route successfully saved!");
            }
            else
                ShowUnSuccessAlert("There is no route to be saved!");
        }

        protected async Task GetRoutes()
        {
            var userId = await LocalStorageManagerService.GetPermanentItemAsync("UserId");
            CityFilter cityFilter = new CityFilter { UserId = userId };
            var result = (await CitiesDataService.GetRoutes(cityFilter.GetFilter()));
            if (result != null)
                LastRoutes = result;
            else
                ShowUnSuccessAlert("You have no routes saved in our DB!");
            StateHasChanged();
        }

        protected async Task GetUnsavedRoute()
        {
            var serializedCities = await LocalStorageManagerService.GetPermanentItemAsync("Cities");
            if (serializedCities != null)
            {
                this.Cities = JsonConvert.DeserializeObject<List<City>>(serializedCities);
                await ShowRoute();
                ShowSuccessAlert("The route was successfully restored!");
            }
            else
                ShowUnSuccessAlert("You have no route cached in your browser!");
        }

        private void ShowSuccessAlert(string message)
        {
            ShowSuccessAlert_Prop = true;
            SuccessAlertMessage = message;
            StateHasChanged();
            SuccessAlertTimer.Enabled = true;
        }

        private void ShowUnSuccessAlert(string message)
        {
            ShowUnSuccessAlert_Prop = true;
            UnSuccessAlertMessage = message;
            StateHasChanged();
            UnSuccessAlertTimer.Enabled = true;
        }

        protected async Task RestaurantSelected(GoogleTextSearchDTO restaurant)
        {
            this.Cities[Cities.Count() - 1].SelectedRestaurant = restaurant;
            this.RestaurantSearches.Clear();
            if (this.MuseumSearches.Count == 0 && this.RestaurantSearches.Count == 0)
            {
                var serializedCities = JsonConvert.SerializeObject(this.Cities);
                await LocalStorageManagerService.DeletePermanentItemAsync("Cities");
                await LocalStorageManagerService.SavePermanentItemAsync("Cities", serializedCities);
            }
            await JSRuntime.InvokeVoidAsync("hideLocation");
            ShowSuccessAlert("Restaurant selected");
            StateHasChanged();
        }

        protected async Task MuseumSelected(GoogleTextSearchDTO museum)
        {
            this.Cities[Cities.Count() - 1].SelectedMuseum = museum;
            this.MuseumSearches.Clear();
            //Check if user selected all from the tables
            if (this.MuseumSearches.Count == 0 && this.RestaurantSearches.Count == 0)
            {
                var serializedCities = JsonConvert.SerializeObject(this.Cities);
                await LocalStorageManagerService.DeletePermanentItemAsync("Cities");
                await LocalStorageManagerService.SavePermanentItemAsync("Cities", serializedCities);
            }
            await JSRuntime.InvokeVoidAsync("hideLocation");
            ShowSuccessAlert("Museum selected");
            StateHasChanged();
        }

        protected async Task RouteSelected(Tuple<DateTime, List<City>> selectedRoute)
        {
            this.Cities = selectedRoute.Item2;
            this.LastRoutes.Cities.Clear();
            await this.ShowRoute();
            ShowSuccessAlert("Route selected");
        }

        protected async Task ShowLocation(GoogleTextSearchDTO place)
        {
            await JSRuntime.InvokeVoidAsync("showLocation", new LocationDTO { lat = place.geometry.location.lat, lng = place.geometry.location.lng });
        }

        protected async Task HideLocation(GoogleTextSearchDTO place)
        {
            await JSRuntime.InvokeVoidAsync("hideLocation");
        }

        public async Task AddCityDialog_OnDialogClose(Map_AddCity map_AddCity)
        {
            this.RestaurantSearches = map_AddCity.RestaurantSearches;
            this.MuseumSearches = map_AddCity.MuseumSearches;
            this.Cities.Add(map_AddCity.City);
            this.Cities[this.Cities.Count - 1].UserId = await LocalStorageManagerService.GetPermanentItemAsync("UserId");
            if (this.MuseumSearches.Count == 0 && this.RestaurantSearches.Count == 0)
            {
                var serializedCities = JsonConvert.SerializeObject(this.Cities);
                await LocalStorageManagerService.DeletePermanentItemAsync("Cities");
                await LocalStorageManagerService.SavePermanentItemAsync("Cities", serializedCities);
            }
            if (this.MuseumSearches.Count == 0 && map_AddCity.City.NeedsMuseum)
                ShowUnSuccessAlert("Sorry! We couldn't find any museums in your area");
            if (this.RestaurantSearches.Count == 0 && map_AddCity.City.NeedsRestaurant)
                ShowUnSuccessAlert("Sorry! We couldn't find any restaurants in your area");
            ShowSuccessAlert("The city was successfully added to the route!");
        }

        public LocationDTO GetStartPointGeoCoordinates()
        {
            return Cities.Where(x => x.StartPoint)
                         .Select(x => x.Location)
                         .FirstOrDefault();
        }

        public LocationDTO GetDestinationPointGeoCoordinates()
        {
            return Cities.Where(x => x.DestinationPoint)
                         .Select(x => x.Location)
                         .FirstOrDefault();
        }

        public IList<LocationDTO> GetIntermediatePointsGeoCoordinates()
        {
            return Cities.Where(x => !x.DestinationPoint && !x.StartPoint)
                         .Select(x => x.Location)
                         .ToList();
        }
    }
}
