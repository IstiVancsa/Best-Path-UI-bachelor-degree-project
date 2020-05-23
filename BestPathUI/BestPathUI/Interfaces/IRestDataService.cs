using System.Threading.Tasks;
using System.Collections.Generic;
using Models;
using Models.Interfaces;

namespace Interfaces
{
    public interface IRestDataService<TModel, DTOModel>
        where TModel : class, IBaseModel
        where DTOModel : class, IBaseDTO
    {
        Task<bool> AddItemAsync(TModel item);
        Task<bool> UpdateItemAsync(TModel item);
        Task<bool> DeleteItemAsync(int id);
        Task<TModel> GetItemAsync(int id);
        Task<IList<TModel>> GetItemsAsync();
        Task<IList<TModel>> GetItemsAsync(string filters);
        Task<IList<BaseModel>> GetBaseNameModel();
        Task<TResult> ReturnGetHttp<TResult>(string url);
    }
}

