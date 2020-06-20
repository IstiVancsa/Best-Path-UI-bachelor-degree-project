using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models.DTO;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness
{
    public static class AStarAlgorithm
    {
        public static GoogleDistanceDTO Result { get; set; }
        [Inject]
        public static IJSRuntime JSRuntime { get; set; }
        public static List<AStarCity> Cities { get; set; }
        private static void Constructor(List<City> cities)
        {
            foreach(var city in cities)
            {
                Cities.Add(new AStarCity(city));
            }
        }
        public static List<City> GetOrderedCities(List<City> cities)
        {
            Constructor(cities);
            ApplyAStarAlgorithm();
            return Cities.Select(x => x.City).ToList();
        }
        private static void ApplyAStarAlgorithm()
        {
            var finalCitiesList = new List<AStarCity>();
            var originCity = GetOriginCity();
            if (originCity != null)
                finalCitiesList.Add(originCity);
            else
                finalCitiesList = null;

            var intermediateCities = GetIntermediateCity();
            //here goes A* alg

            var destinationCity = GetDestinationCity();
            if (destinationCity != null)
                finalCitiesList.Add(destinationCity);
            else
                finalCitiesList = null;
        }
        private static List<AStarCity> GetUnvisitedCities()
        {
            return Cities.Where(x => !x.Visited).ToList();
        }
        private static AStarCity GetOriginCity()
        {
            return Cities.FirstOrDefault(x => x.IsOrigin);
        }
        private static AStarCity GetDestinationCity()
        {
            return Cities.FirstOrDefault(x => x.IsDestination);
        }
        private static List<AStarCity> GetIntermediateCity()
        {
            return Cities.Where(x => !x.IsDestination && !x.IsOrigin).ToList();
        }
        private static int GetScore(AStarCity start, AStarCity end)
        {
            return 0;
        }
        private async static Task<GoogleDistanceDTO> GetTravelInfo(LocationDTO start, LocationDTO end)
        {
            await JSRuntime.InvokeVoidAsync("getDistance", start, end);
            return Result;
        }
        [JSInvokable]
        public static void SetResult(GoogleDistanceDTO result)
        {
            Result = result;
        }
    }
}
