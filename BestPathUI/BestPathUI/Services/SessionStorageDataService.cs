using Interfaces;
using Microsoft.JSInterop;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class SessionStorageDataService : ISessionStorageDataService
    {
        private readonly IJSRuntime _jSRuntime;

        public event EventHandler<ChangedEventArgs> Changed;
        public event EventHandler<ChangingEventArgs> Changing;

        public SessionStorageDataService(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task ClearAsync()
        {
            await _jSRuntime.InvokeAsync<object>("sessionStorage.clear");
        }

        

        public async Task<T> GetItemAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var serializedData = await _jSRuntime.InvokeAsync<string>("sessionStorage.getItem", key);

            if (serializedData == null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(serializedData);
        }


        public async Task<string> GetKeyAsync(int index)
        {
            return await _jSRuntime.InvokeAsync<string>("sessionStorage.key", index);
        }

        public async Task<int> LengthAsync()
        {
            return await _jSRuntime.InvokeAsync<int>("eval", "sessionStorage.length");
        }

        public async Task RemoveItemAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            await _jSRuntime.InvokeAsync<object>("sessionStorage.removeItem", key);
        }

        public async Task SetItemAsync(string key, object data)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var e = await RaiseOnChangingAsync(key, data);
            if (e.Cancel) return;

            await _jSRuntime.InvokeAsync<object>("sessionStorage.setItem", key, JsonSerializer.Serialize(data));

            RaiseOnChangingSync(key, e.OldValue, data);
        }

        private async Task<ChangingEventArgs> RaiseOnChangingAsync(string key, object data)
        {
            var e = new ChangingEventArgs
            {
                Key = key,
               
                NewValue = data
            };
            e.OldValue = this.GetItemAsync<object>(key);
            Changing?.Invoke(this, e);
            return e;
        }

        private void RaiseOnChangingSync(string key, object oldValue, object data)
        {
            var e = new ChangingEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = data
            };
            Changing?.Invoke(this, e);
        }
    }
}
