using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models.DTO;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestPathUI.Pages.MapPage
{
    public static class AStarAlgorithm
    {
        public static GoogleDistanceDTO Result { get; set; } = null;
        public static GoogleDistanceDTO _lastResult { get; set; } = null;
        [Inject]
        public static IJSRuntime JSRuntime { get; set; }
        private static List<AStarCity> Cities { get; set; }
        private static DateTime CurrentTime { get; set; }
        private static DateTime _backUpTime { get; set; }
        private static int _segmentsCount { get; set; }
        private static int _timeImportanceCoeficient { get; set; }
        public static bool NoRouteFound { get; set; } = false;
        private static void Constructor(IList<City> cities, IJSRuntime jSRuntime, DateTime startTime)
        {
            Cities = new List<AStarCity>();
            JSRuntime = jSRuntime;
            CurrentTime = startTime;
            _backUpTime = startTime;
            _segmentsCount = 0;
            _timeImportanceCoeficient = 3;
            foreach (var city in cities)
            {
                Cities.Add(new AStarCity(city));
            }
        }
        public static async Task<List<LocationDTO>> GetOrderedCities(IList<City> cities, IJSRuntime jSRuntime, DateTime startTime)
        {
            Constructor(cities, jSRuntime, startTime);
            return await ApplyAStarAlgorithm();
        }
        private static async Task<List<LocationDTO>> ApplyAStarAlgorithm()
        {
            var finalCitiesList = new List<LocationDTO>();
            AStarCity currentCity = null;
            var originCity = GetOriginCity();
            if (originCity != null)
            {
                originCity.Visited = true;
                Cities.FirstOrDefault(x => x.City.Id == originCity.City.Id).Visited = true;
                finalCitiesList.Add(GetLocation(originCity));
                currentCity = originCity;
            }
            else
            {
                finalCitiesList = null;
                return null;
            }

            while (AreNotVisitedCities())
            {
                var unvisitedCities = GetUnvisitedIntermediateCities();
                var scoredUnvisitedCities = await GetScores(currentCity, unvisitedCities);
                if(scoredUnvisitedCities.Any(x => x.Score == 0) && _timeImportanceCoeficient < 99)
                {
                    ResetAlgorithm();
                    _timeImportanceCoeficient++;
                }
                else
                    if(!scoredUnvisitedCities.Any(x => x.Score == 0))
                    {
                        var highestScoredCity = GetHighestScoredCity(scoredUnvisitedCities);
                        //set highestScoredCity to visited
                        Cities.FirstOrDefault(x => x.City.Id == highestScoredCity.City.City.Id).Visited = true;
                        finalCitiesList.Add(GetLocation(highestScoredCity.City));
                        currentCity = highestScoredCity.City;
                    }
                    else
                    {
                        NoRouteFound = true;
                        return null;
                    }
                        
            }

            var destinationCity = GetDestinationCity();
            if (destinationCity != null)
                finalCitiesList.Add(GetLocation(destinationCity));
            else
                finalCitiesList = null;
            return finalCitiesList;
        }

        private static void ResetAlgorithm()
        {
            Cities.Where(x => !x.IsOrigin && !x.IsDestination).ToList().ForEach(x => x.Visited = false);
            CurrentTime = _backUpTime;
        }

        private static List<AStarCity> GetUnvisitedIntermediateCities()
        {
            return Cities.Where(x => !x.Visited && !x.IsDestination && !x.IsOrigin).ToList();
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
        private static async Task<double> CalculateScore(AStarCity start, AStarCity end)
        {
            var startLocation = GetLocation(start);
            var endLocation = GetLocation(end);
            await GetTravelInfo(startLocation, endLocation);
            while(Result == null)
                await Task.Delay(25);
            double distanceScore = 1 / (double)Result.distance.value;
            CurrentTime = CurrentTime.AddSeconds(Result.duration.value);
            if (CurrentTime > end.City.ArrivingTime)
                return 0;
            double timeScore = 1 / (end.City.ArrivingTime.Subtract(CurrentTime).TotalSeconds) * _timeImportanceCoeficient;
            Result = null;
            return distanceScore + timeScore;
        }
        private static LocationDTO GetLocation(AStarCity city)
        {
            if (city.City.NeedsMuseum)
                return city.City.SelectedMuseum.geometry.location;
            else
                if (city.City.NeedsRestaurant)
                return city.City.SelectedRestaurant.geometry.location;
            else
                return city.City.Location;
        }
        private static async Task<List<ScoredAStarCity>> GetScores(AStarCity start, List<AStarCity> neighbors)
        {
            List<ScoredAStarCity> highScores = new List<ScoredAStarCity>();
            foreach (var neighbor in neighbors)
            {
                double score = await CalculateScore(start, neighbor);
                highScores.Add(new ScoredAStarCity { Score = score, City = neighbor });
            }
            return highScores;
        }
        private static ScoredAStarCity GetHighestScoredCity(List<ScoredAStarCity> scoredAStarCities)
        {
            return scoredAStarCities.OrderByDescending(x => x.Score).ToList()[0];
        }
        private async static Task<GoogleDistanceDTO> GetTravelInfo(LocationDTO start, LocationDTO end)
        {
            await JSRuntime.InvokeVoidAsync("getDistance", start, end);
            return Result;
        }
        private static bool AreNotVisitedCities()
        {
            return GetUnvisitedIntermediateCities().Count > 0;
        }
        [JSInvokable]
        public static void SetResult(GoogleDistanceDTO result)
        {
            if (_segmentsCount % 2 == 0)
                _lastResult = result;
            else
            {
                if (result.distance.value > _lastResult.distance.value)
                    Result = result;
                else
                    Result = _lastResult;
            }
            _segmentsCount++;
        }
    }
}
