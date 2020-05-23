using Models.DTO;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ICitiesDataService : IRestDataService<City, CityDTO>
    {
        Task<bool> SavePathAsync(IEnumerable<City> cities);
        Task<GetLastRouteResult> GetRoutes(string filters);
    }
}
