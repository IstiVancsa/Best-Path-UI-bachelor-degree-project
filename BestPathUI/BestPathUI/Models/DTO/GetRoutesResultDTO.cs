using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.DTO
{
    public class GetRoutesResultDTO : BaseTokenizedDTO
    {
        public List<Tuple<DateTime, List<City>>> Routes { get; set; }
        public GetRoutesResultDTO()
        {
            this.Routes = new List<Tuple<DateTime, List<City>>>();
        }
    }
}
