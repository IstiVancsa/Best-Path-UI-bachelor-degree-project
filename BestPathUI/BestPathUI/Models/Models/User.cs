using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Models
{
    public class User
    {
        public string Id { get; set; }
        public Guid Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
