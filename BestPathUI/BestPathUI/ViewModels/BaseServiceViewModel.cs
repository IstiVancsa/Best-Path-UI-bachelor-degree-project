using Models.Interfaces;
using Interfaces;

namespace ViewModels
{
    public abstract class BaseServiceViewModel<TModel, DTOModel, IServiceInterface> : BaseViewModel
        where TModel : class, IBaseModel, new()
        where DTOModel : class, IBaseDTO, new()
        where IServiceInterface : class, IRestDataService<TModel, DTOModel>
    {
        protected IServiceInterface CurrentService;
        public BaseServiceViewModel(IServiceInterface service)
        {
            this.CurrentService = service;
        }
    }
}
