using Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IGoogleDataService
    {
        Task<GoogleTextSearchResultsDTO> TextSearch(string searchText, LocationDTO locationDTO);
    }
}
