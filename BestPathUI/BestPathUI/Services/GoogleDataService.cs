using Models.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Interfaces;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class GoogleDataService : IGoogleDataService
    {
        private HttpClient _httpClient;
        public IConfiguration Configuration { get; }

        public GoogleDataService(HttpClient httpClient, IConfiguration configuration)
        {
            this._httpClient = httpClient;
            Configuration = configuration;
        }
        public async Task<GoogleTextSearchResultsDTO> TextSearch(string searchText, LocationDTO locationDTO)
        {
            var item = new GoogleTextSearchResultsDTO();
            var uri = new Uri(Configuration["APPPaths:GoogleSearch"] + searchText + "&location=" + locationDTO.lat.ToString() + "%2C" + locationDTO.lng.ToString() + "&radius=5000&key=AIzaSyAIztt-WDuygbYykfzV8akJ3DyR-jrhJRY");
            try
            {
                var response = await this._httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    item = JsonConvert.DeserializeObject<GoogleTextSearchResultsDTO>(content);
                    item.results = item.results.Where(x => Math.Abs(x.geometry.location.lat - locationDTO.lat) + Math.Abs(x.geometry.location.lng - locationDTO.lng) < 0.1).ToList();
                }
            }
            catch (Exception ex)
            {
                var dsad = ex.Message;
            }
            return item;
        }
    }
}
