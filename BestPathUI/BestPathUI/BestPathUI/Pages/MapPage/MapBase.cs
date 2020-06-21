using BestPathUI.Pages.Components;
using Bussiness;
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
        protected DeleteRouteDialog DeleteRouteDialog { get; set; }
        public IList<GoogleTextSearchDTO> RestaurantSearches { get; set; } = new List<GoogleTextSearchDTO>();
        public IList<GoogleTextSearchDTO> MuseumSearches { get; set; } = new List<GoogleTextSearchDTO>();
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public GetRoutesResultDTO LastRoutes { get; set; }
        public bool ShowSuccessAlert_Prop { get; set; }
        public bool ShowUnSuccessAlert_Prop { get; set; }
        public string SuccessAlertMessage { get; set; }
        public string UnSuccessAlertMessage { get; set; }
        public Timer SuccessAlertTimer { get; set; }
        public Timer UnSuccessAlertTimer { get; set; }
        protected override async Task OnInitializedAsync()
        {
            Cities = new List<City>();
            LastRoutes = new GetRoutesResultDTO();
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
        protected int RouteDistance { get; set; }
        protected int RouteDuration { get; set; }
        protected string ProcessedRouteDistance
        {
            get
            {
                if (RouteDistance < 1000)
                    return RouteDistance.ToString() + " m";
                else
                if (RouteDistance < 2000)
                    return (Convert.ToInt32(RouteDistance / 1000)).ToString() + " km " + (RouteDistance % 1000).ToString() + " m";
                else
                    return (Convert.ToInt32(RouteDistance / 1000)).ToString() + " kms " + (RouteDistance % 1000).ToString() + " m";
            }
        }
        protected string ProcessedRouteDuration
        {
            get
            {
                if (RouteDuration < 60)
                    return RouteDuration.ToString() + " s";
                else
               if (RouteDuration < 120)
                    return (Convert.ToInt32(RouteDuration / 60)).ToString() + " min " + (RouteDuration % 60).ToString() + " s";
                else
               if (RouteDuration < 3600)
                    return (Convert.ToInt32(RouteDuration / 60)).ToString() + " mins " + (RouteDuration % 60).ToString() + " s";
                else
                   if (RouteDuration < 7200)
                    return (Convert.ToInt32(RouteDuration / 3600)).ToString() + " hour " + (Convert.ToInt32(RouteDuration % 3600 / 60)).ToString() + " mins " + (RouteDuration % 60).ToString() + " s";
                else
                    return (Convert.ToInt32(RouteDuration / 3600)).ToString() + " hours " + (Convert.ToInt32(RouteDuration % 3600 / 60)).ToString() + " mins " + (RouteDuration % 60).ToString() + " s";
            }
        }
        private int _segmentsCount { get; set; }
        private GoogleDistanceDTO _lastdistance { get; set; }
        public bool AStarRoute { get; set; } = false;
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
            AddCityDialog.Show(AStarRoute);
        }

        protected async Task ShowRoute(bool getOptimizedRoute = false)
        {
            await JSRuntime.InvokeVoidAsync("removeDirections");
            var startPoint = GetStartPointGeoCoordinates();
            var endPoint = GetDestinationPointGeoCoordinates();
            var intermediatePoints = GetIntermediatePointsGeoCoordinates();
            if (startPoint != null && endPoint != null)
                await JSRuntime.InvokeVoidAsync("showRoute", startPoint, endPoint, intermediatePoints, getOptimizedRoute);
            else
                if (startPoint == null)
                ShowUnSuccessAlert("You need to select a start point before showing the path");
            else
                ShowUnSuccessAlert("You need to select a destination point before showing the path");
        }

        protected async Task ShowAStarRoute()
        {
            await JSRuntime.InvokeVoidAsync("removeDirections");

            var result = await AStarAlgorithm.GetOrderedCities(this.Cities, this.JSRuntime, DateTime.Now);

            if (result != null)
            {
                var startPoint = result[0];
                var endPoint = result[result.Count - 1];
                var middlePoints = result.Where(x => result.FindIndex(y => y == x) != 0 && result.FindIndex(y => y == x) != result.Count -1).ToList();
                await JSRuntime.InvokeVoidAsync("showRoute", startPoint, endPoint, middlePoints, false);
            }
            else
                ShowUnSuccessAlert("You need to select a start point and a destination point before showing the path.");
        }
        protected void AttemptToDeleteRoute()
        {
            DeleteRouteDialog.Show();
        }

        protected async Task NewRoute()
        {
            this.Cities.Clear();
            RouteDistance = 0;
            RouteDuration = 0;
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
            if (result?.Routes.Count > 0)
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
            RouteDistance = 0;
            RouteDuration = 0;
            this.Cities = selectedRoute.Item2;
            this.LastRoutes.Routes.Clear();
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
            map_AddCity.City.CityOrder = this.Cities.Count;
            map_AddCity.City.UserId = await LocalStorageManagerService.GetPermanentItemAsync("UserId");

            this.Cities.Add(map_AddCity.City);
            if (this.MuseumSearches.Count == 0 && this.RestaurantSearches.Count == 0)
            {
                var serializedCities = JsonConvert.SerializeObject(this.Cities);
                await LocalStorageManagerService.DeletePermanentItemAsync("Cities");
                await LocalStorageManagerService.SavePermanentItemAsync("Cities", serializedCities);
            }
            if (this.MuseumSearches.Count == 0 && map_AddCity.City.NeedsMuseum)
            {
                if (this.RestaurantSearches.Count == 0 && map_AddCity.City.NeedsRestaurant)
                    ShowUnSuccessAlert("Sorry! We couldn't find any restaurants, nor any museums in your area");
                else
                    ShowUnSuccessAlert("Sorry! We couldn't find any museums in your area");
            }
            else
                if (this.RestaurantSearches.Count == 0 && map_AddCity.City.NeedsRestaurant)
                ShowUnSuccessAlert("Sorry! We couldn't find any restaurants in your area");
            ShowSuccessAlert("The city was successfully added to the route!");
        }

        public LocationDTO GetStartPointGeoCoordinates()
        {
            var startLocation = Cities.Where(x => x.StartPoint)
                         .FirstOrDefault();
            if (startLocation != null)
            {
                if (startLocation.SelectedRestaurant != null)
                    return startLocation.SelectedRestaurant.geometry.location;
                else
                    if (startLocation.SelectedMuseum != null)
                    return startLocation.SelectedMuseum.geometry.location;
                else
                    return startLocation.Location;
            }
            else
                return null;
        }

        public LocationDTO GetDestinationPointGeoCoordinates()
        {
            var destinationLocation = Cities.Where(x => x.DestinationPoint)
                         .FirstOrDefault();

            if (destinationLocation != null)
            {
                if (destinationLocation.SelectedRestaurant != null)
                    return destinationLocation.SelectedRestaurant.geometry.location;
                else
                if (destinationLocation.SelectedMuseum != null)
                    return destinationLocation.SelectedMuseum.geometry.location;
                else
                    return destinationLocation.Location;
            }
            else
                return null;
        }

        public IList<LocationDTO> GetIntermediatePointsGeoCoordinates()
        {
            var intermediateCities = Cities.Where(x => !x.DestinationPoint && !x.StartPoint)
                         .ToList();
            IList<LocationDTO> finalCoordinates = new List<LocationDTO>();
            foreach (var city in intermediateCities)
            {
                if (city.SelectedRestaurant != null)
                    finalCoordinates.Add(city.SelectedRestaurant.geometry.location);
                if (city.SelectedMuseum != null)
                    finalCoordinates.Add(city.SelectedMuseum.geometry.location);
                if (city.SelectedRestaurant == null && city.SelectedMuseum == null)
                    finalCoordinates.Add(city.Location);
            }
            return finalCoordinates;
        }

        [JSInvokable]
        public void SetGoogleDistance(GoogleDistanceDTO distance)
        {
            if (_segmentsCount % 2 == 0)
                _lastdistance = distance;
            else
            {
                if (_lastdistance.distance.value > distance.distance.value)
                {
                    RouteDistance += distance.distance.value;
                    RouteDuration += distance.duration.value;
                }
                else
                {
                    RouteDistance += _lastdistance.distance.value;
                    RouteDuration += _lastdistance.duration.value;
                }
            }
            _segmentsCount++;
            StateHasChanged();
        }
    }
}
