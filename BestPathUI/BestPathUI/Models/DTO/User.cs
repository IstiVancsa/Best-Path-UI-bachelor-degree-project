using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DTO
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}
