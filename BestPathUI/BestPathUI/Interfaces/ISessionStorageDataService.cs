using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ISessionStorageDataService
    {
        Task ClearAsync();
        Task<T> GetItemAsync<T>(string key);
        Task<string> GetKeyAsync(int index);
        Task RemoveItemAsync(string key);
        Task<int> LengthAsync();
        Task SetItemAsync(string key, object data);
        event EventHandler<ChangedEventArgs> Changed;
        event EventHandler<ChangingEventArgs> Changing;
    }
}
