using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewelry_Shop
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool isAdmin { get; set; }

        public User(string username)
        {
            this.username = username;
        }
    }
}
