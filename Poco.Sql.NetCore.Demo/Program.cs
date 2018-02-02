using System;
using System.Collections.Generic;
using System.Diagnostics;
using Poco.Sql.NetCore.Exceptions;
using Poco.Sql.NetCore.Test.Models;
using Poco.Sql.NetCore.Test.Models.Mappings;
using Poco.Sql.NetCore.UnitTests;
using UnitTestRunner;

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
            Console.WriteLine("Written by Ophir Oren. All rights reserved © Ophir Oren 2015-2018.");
            Console.WriteLine("Released under MIT license.");
            Console.WriteLine("http://www.webe.co.il");
            Console.WriteLine("");
            Console.ResetColor();

            TestRunner testRunner = new TestRunner();
            testRunner.RunTestClass(typeof(PocoSqlTests));
            
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

            var user = Helper.GetUser();
            var vUser = Helper.GetVUser();

            Stopwatch swStopwatch = new Stopwatch();
            swStopwatch.Start();
            var currentlyTestingSql = user.PocoSql().Select().Where<User>(u => u.Name.Equals("ophir")).ToString();
            swStopwatch.Stop();

            // Currently testing
            Console.WriteLine(
                Environment.NewLine +
                Environment.NewLine + "~~~~~~~~~~~~~~~~~~~~~~~~~" + Environment.NewLine + "Currently developing: " + Environment.NewLine + currentlyTestingSql);
            Console.WriteLine("Time elapsed: {0}", swStopwatch.Elapsed);
            
            Console.ReadLine();
        }
    }
}