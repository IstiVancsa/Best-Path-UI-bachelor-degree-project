using Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.Interfaces;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Bussiness;
using System.IO;
using Models.DTO;

namespace Services
{
    public class RestDataService<TModel, DTOModel> : IRestDataService<TModel, DTOModel>
                where TModel : class, IBaseModel
                where DTOModel : class, IBaseDTO
    {
        public Func<DTOModel, TModel> GetByFilterSelector = null;
        private HttpClient _httpClient;
        private IJSRuntime _jSRuntime;
        public IConfiguration Configuration { get; }
        public string UrlApi = "";
        public ILocalStorageManagerService LocalStorageManagerService { get; set; }
        public RestDataService(HttpClient httpClient, IConfiguration configuration, IJSRuntime JSRuntime, ILocalStorageManagerService localStorageManagerService, string partialUrl = null)
        {
            if (string.IsNullOrEmpty(partialUrl))
                partialUrl = typeof(TModel).Name.Replace("Model", "");

            this.LocalStorageManagerService = localStorageManagerService;
            Configuration = configuration;
            this.UrlApi = Configuration["APPPaths:BestPathAPI"] + partialUrl + "/";
            this._httpClient = httpClient;
            _jSRuntime = JSRuntime;
            GetToken();
        }
        public async Task<string> GetToken()
        {
            return await LocalStorageManagerService.GetPermanentItemAsync("Token");
        }
        public async Task<bool> AddItemAsync(TModel item)
        {
            bool result = true;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await this.GetToken());

                var content = new StringContent(JsonConvert.SerializeObject(item.GetDTO()), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(this.UrlApi, content);

                var token = await response.Content.ReadAsStringAsync();

                await LocalStorageManagerService.UpdatePermanentItemAsync("Token", token);

                result = response.IsSuccessStatusCode;
            }

            return result;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.UrlApi + id);
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + await this.GetToken());
            request.Method = "DELETE";
            request.ContentType = "application/json";
            var response = (await request.GetResponseAsync()) as HttpWebResponse;
            var contentStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(contentStream, Encoding.UTF8);
            var token = readStream.ReadToEnd();
            await LocalStorageManagerService.UpdatePermanentItemAsync("Token", token);
            response.Close();
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<TModel> GetItemAsync(int id)
        {
            return (await this.ReturnGetHttp<List<DTOModel>>(this.UrlApi + "GetByFilter/?Id=" + id)).Select(GetByFilterSelector).FirstOrDefault();
        }

        public async Task<IList<TModel>> GetItemsAsync()
        {
            return (await this.ReturnGetHttp<List<DTOModel>>(this.UrlApi + "GetByFilter/")).Select(GetByFilterSelector).ToList();
        }

        public async Task<IList<TModel>> GetItemsAsync(string filters)
        {
            return (await this.ReturnGetHttp<List<DTOModel>>(this.UrlApi + "GetByFilter/" + filters)).Select(GetByFilterSelector).ToList();
        }

        public async Task<IList<BaseModel>> GetBaseNameModel()
        {
            return await this.ReturnGetHttp<List<BaseModel>>(this.UrlApi + "GetBaseModel/");
        }



        public async Task<bool> UpdateItemAsync(TModel item)
        {
            var uri = new Uri(string.Format(this.UrlApi));

            var serializedItem = JsonConvert.SerializeObject(item.GetDTO());
            var content = new StringContent(serializedItem, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await this.GetToken());
            var response = await this._httpClient.PutAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                await LocalStorageManagerService.UpdatePermanentItemAsync("Token", token);
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<TResult> ReturnGetHttp<TResult>(string url)
        {
            TResult items = default(TResult);

            var uri = new Uri(string.Format(url));
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await this.GetToken());
                var response = await this._httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    items = JsonConvert.DeserializeObject<TResult>(content);
                    var token = items as BaseTokenizedDTO;
                    if(token != null)
                    {
                        await LocalStorageManagerService.UpdatePermanentItemAsync("Token", token.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                var dsad = ex.Message;
            }

            return items;
        }
    }
}
