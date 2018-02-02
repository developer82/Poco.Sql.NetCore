using System;
using System.Collections.Generic;
using System.Text;

namespace Poco.Sql.NetCore.Test.Models
{
    public class Helper
    {
        public static User GetUser()
        {
            return new User()
            {
                UserId = 1,
                Age = 32,
                Name = "Ophir",
                Birthday = new DateTime(1982, 5, 6)
            };
        }

        public static VUser GetVUser()
        {
            return new VUser()
            {
                UserId = 1,
                Age = 32,
                Name = "Ophir",
                Birthday = new DateTime(1982, 5, 6)
            };
        }

        public static List<Order> GetListOfOrders()
        {
            var user = GetUser();

            return new List<Order>()
            {
                new Order() { UserId = 1, User = user, ItemName = "Item 1", OrderId = 1, Quantity = 5 },
                new Order() { UserId = 1, User = user, ItemName = "Item 2", OrderId = 1, Quantity = 4 },
                new Order() { UserId = 1, User = user, ItemName = "Item 3", OrderId = 1, Quantity = 3 },
                new Order() { UserId = 1, User = user, ItemName = "Item 4", OrderId = 1, Quantity = 2 },
                new Order() { UserId = 1, User = user, ItemName = "Item 5", OrderId = 1, Quantity = 1 }
            };
        }
    }
}
