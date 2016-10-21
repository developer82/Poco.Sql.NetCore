using System;
using System.Collections.Generic;

namespace Poco.Sql.NetCore.Test.Models
{
    public class VUser
    {
        public VUser()
        {
            Orders = new List<Order>();
        }

        public int UserId { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public IList<Order> Orders { get; set; }
    }
}
