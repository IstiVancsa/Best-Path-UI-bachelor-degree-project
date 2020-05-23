using Bussiness;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Models.DTO.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class AuthenticationDataService : IAuthenticationDataService
    {
        public string UrlApi = "";
        public IConfiguration Configuration { get; }
        public ILocalStorageManagerService LocalStorageManagerService { get; }

        private HttpClient _httpClient;
        public AuthenticationDataService(HttpClient httpClient, IConfiguration configuration, ILocalStorageManagerService localStorageManagerService)
        {
            Configuration = configuration;
            LocalStorageManagerService = localStorageManagerService;
            this.UrlApi = Configuration["APPPaths:BestPathAPI"] + "Accounts/";

            this._httpClient = httpClient;
        }

        public async Task<RegisterResultDTO> Register(RegisterRequestDTO item)
        {
            var result = new RegisterResultDTO { Successful = false };
            var request = new HttpRequestMessage(HttpMethod.Post, this.UrlApi + "Register");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            request.Content = content;
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request, CancellationToken.None);

            string str = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<RegisterResultDTO>(str);
            return result;
        }

        public async Task<LoginResultDTO> Login(LoginRequestDTO item)
        {
            var result = new LoginResultDTO { Successful = false };
            var request = new HttpRequestMessage(HttpMethod.Post, this.UrlApi + "Login");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            request.Content = content;
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request, CancellationToken.None);

            string str = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<LoginResultDTO>(str);
            await LocalStorageManagerService.SavePermanentItemAsync("Token", result.Token);
            await LocalStorageManagerService.SavePermanentItemAsync("UserId", result.UserId);
            return result;
        }
    }
}
