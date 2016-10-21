using System;
using System.Collections.Generic;
using System.Diagnostics;
using Poco.Sql.NetCore.Exceptions;
using Poco.Sql.NetCore.Test.Models;
using Poco.Sql.NetCore.Test.Models.Mappings;

namespace Poco.Sql.NetCore.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO: Bug: Values are injected in WHERE statement when they should be parameterized

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@" ____   __    ___  __     ____   __   __   ");
            Console.WriteLine(@"(  _ \ /  \  / __)/  \   / ___) /  \ (  )  ");
            Console.WriteLine(@" ) __/(  O )( (__(  O )_ \___ \(  O )/ (_/\");
            Console.WriteLine(@"(__)   \__/  \___)\__/(_)(____/ \__\)\____/");
            Console.WriteLine("");
            Console.WriteLine("Poco.Sql");
            Console.WriteLine("Written by Ophir Oren. All rights reserved © Ophir Oren 2015.");
            Console.WriteLine("Released under MIT license.");
            Console.WriteLine("http://www.webe.co.il");
            Console.WriteLine("");
            Console.ResetColor();

            // PocoSql configuration
            Configuration.Initialize(config =>
            {
                config.PluralizeTableNames();
                config.SetStoreProcedurePrefix("stp_");
                //config.ShowComments();
                //config.InjectValuesInQueies();
                //config.SelectFullGraph(); // TODO: not completed yet

                config.AddMap(new UserMap());
                config.AddMap(new OrderMap());
                config.AddMap(new VUserMap());
            });

            string sql;

            var user = new User()
            {
                UserId = 1,
                Age = 32,
                Name = "Ophir",
                Birthday = new DateTime(1982, 5, 6)
            };

            var vuser = new VUser()
            {
                UserId = 1,
                Age = 32,
                Name = "Ophir",
                Birthday = new DateTime(1982, 5, 6)
            };

            var orders = new List<Order>()
            {
                new Order() { UserId = 1, User = user, ItemName = "Item 1", OrderId = 1, Quantity = 5 },
                new Order() { UserId = 1, User = user, ItemName = "Item 2", OrderId = 1, Quantity = 4 },
                new Order() { UserId = 1, User = user, ItemName = "Item 3", OrderId = 1, Quantity = 3 },
                new Order() { UserId = 1, User = user, ItemName = "Item 4", OrderId = 1, Quantity = 2 },
                new Order() { UserId = 1, User = user, ItemName = "Item 5", OrderId = 1, Quantity = 1 }
            };


            Stopwatch swStopwatch = new Stopwatch();
            swStopwatch.Start();
            var currentlyTestingSql = user.PocoSql().Select().Where<User>(u => u.Name.Equals("ophir")).ToString();
            swStopwatch.Stop();

            sql = user.PocoSql().Select().ToString();
            Console.WriteLine("user.PocoSql().Select()");
            Console.WriteLine(sql + Environment.NewLine);

            sql = user.PocoSql().Insert().ToString();
            Console.WriteLine("user.PocoSql().Insert()");
            Console.WriteLine(sql + Environment.NewLine);

            sql = user.PocoSql().Update().ToString();
            Console.WriteLine("user.PocoSql().Update()");
            Console.WriteLine(sql + Environment.NewLine);

            sql = user.PocoSql().Delete().ToString();
            Console.WriteLine("user.PocoSql().Delete()");
            Console.WriteLine(sql + Environment.NewLine);

            sql = vuser.PocoSql().Select().ToString();
            Console.WriteLine("vuser.PocoSql().Select()");
            Console.WriteLine(sql + Environment.NewLine);

            sql = vuser.PocoSql().Select().Where<VUser>(u => u.UserId == 1).ToString();
            Console.WriteLine("vuser.PocoSql().Select()");
            Console.WriteLine(sql + Environment.NewLine);

            sql = vuser.PocoSql().Select().Where<VUser>(u => u.Birthday > DateTime.Now.AddYears(-50) && u.Birthday < DateTime.Now).ToString(); // TODO: this query is not performing correctly
            Console.WriteLine("vuser.PocoSql().Select()");
            Console.WriteLine(sql + Environment.NewLine);

            try
            {
                sql = vuser.PocoSql().Insert().ToString();
            }
            catch (CantUpdateVirtualException ex)
            {
                Console.WriteLine("The following exception is expected");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }


            // Currently testing
            Console.WriteLine(
                Environment.NewLine +
                Environment.NewLine + "~~~~~~~~~~~~~~~~~~~~~~~~~" + Environment.NewLine + "Currently developing: " + Environment.NewLine + currentlyTestingSql);
            Console.WriteLine("Time elapsed: {0}", swStopwatch.Elapsed);

            Console.ReadLine();
        }
    }
}