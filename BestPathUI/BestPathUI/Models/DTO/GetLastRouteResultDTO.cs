using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.DTO
{
    public class GetLastRouteResult : BaseTokenizedDTO
    {
        public List<Tuple<DateTime, List<City>>> Routes { get; set; }
        public GetLastRouteResult()
        {
            this.Routes = new List<Tuple<DateTime, List<City>>>();
        }
    }
}
