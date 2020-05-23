using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DTO
{
    public class OpeningHourDTO
    {
        public bool open_now { get; set; }// to get the opening and closing hours you need to use Place Details request
    }
}
