using Interfaces;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness
{
    public class LocalStorageManagerService : ILocalStorageManagerService
    {
        private IJSRuntime JSRuntime { get; set; }
        //used to simulate session
        private LocalStorage LocalStorage { get; set; }
        public LocalStorageManagerService(IJSRuntime jSRuntime)
        {
            JSRuntime = jSRuntime;
            LocalStorage = new LocalStorage();
        }

        public async Task DeletePermanentItemAsync(string key)
        {
            await JSRuntime.InvokeAsync<string>("stateManager.remove", key);
        }

        public void DeleteTemporaryItem(string key)
        {
            this.LocalStorage[key] = "";
        }

        public async Task<string> GetPermanentItemAsync(string key)
        {
            return await JSRuntime.InvokeAsync<string>("stateManager.load", key);
        }

        public string GetTemporaryItem(string key) => (string)this.LocalStorage[key];

        public async Task SavePermanentItemAsync(string key, string value)
        {
            await JSRuntime.InvokeVoidAsync("stateManager.save", key, value);
        }

        public void SaveTemporaryItem(string key, string value)
        {
            this.LocalStorage[key] = value;
        }

        public async Task UpdatePermanentItemAsync(string key, string value)
        {
            await this.DeletePermanentItemAsync(key);
            await this.SavePermanentItemAsync(key, value);
        }

        public void UpdateTemporaryItem(string key, string value)
        {
            this.LocalStorage[key] = value;
        }
    }
}
