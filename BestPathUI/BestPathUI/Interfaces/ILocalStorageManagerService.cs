using System.Threading.Tasks;

namespace Interfaces
{
    public interface ILocalStorageManagerService
    {
        Task DeletePermanentItemAsync(string key);
        void DeleteTemporaryItem(string key);
        Task<string> GetPermanentItemAsync(string key);
        string GetTemporaryItem(string key);
        Task SavePermanentItemAsync(string key, string value);
        void SaveTemporaryItem(string key, string value);
        Task UpdatePermanentItemAsync(string key, string value);
        void UpdateTemporaryItem(string key, string value);
    }
}