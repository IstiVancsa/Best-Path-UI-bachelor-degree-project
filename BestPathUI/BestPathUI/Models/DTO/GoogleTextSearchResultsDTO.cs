using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DTO
{
    public class GoogleTextSearchResultsDTO
    {
        public IList<GoogleTextSearchDTO> results { get; set; }
    }
}
