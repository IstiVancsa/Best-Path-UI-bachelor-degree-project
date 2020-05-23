using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DTO.Authentication
{
    public class RegisterResultDTO
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
