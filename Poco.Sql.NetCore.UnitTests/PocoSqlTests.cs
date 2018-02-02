using System;
using System.Reflection.Metadata;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poco.Sql.NetCore.Test.Models;
using Poco.Sql.NetCore.Test.Models.Mappings;

namespace Poco.Sql.NetCore.UnitTests
{
    [TestClass]
    public class PocoSqlTests
    {
        private User _user;
        private VUser _vUser;

        public PocoSqlTests()
        {
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

            _user = Helper.GetUser();
            _vUser = Helper.GetVUser();
        }

        [TestMethod]
        public void Generate_Select_Query_From_Object()
        {
            string sql = _user.PocoSql().Select().ToString();
            sql.Should().Be("select UserId, Age, Name, DateOfBirth as Birthday from Users;");
        }

        [TestMethod]
        public void Generate_Insert_Statement_From_Object()
        {
            var sql = _user.PocoSql().Insert().ToString();
            sql.Should().Be("insert into Users (Age, Name, Birthday) values(@Age, @Name, @Birthday);");
        }

        [TestMethod]
        public void Generate_Update_Statement_From_Object()
        {
            var sql = _user.PocoSql().Update().ToString();
            sql.Should().Be("update Users set Age = @Age, Name = @Name, Birthday = @Birthday where (UserId = @UserId);");
        }

        [TestMethod]
        public void Generate_Delete_Statement_From_Object()
        {
            var sql = _user.PocoSql().Delete().ToString();
            sql.Should().Be("delete from Users where (UserId = @UserId);");
        }

        [TestMethod]
        public void Generate_Select_Query_From_Object_With_Virtual_Mapping()
        {
            var sql = _vUser.PocoSql().Select().ToString();
            sql.Should().Be("select UserId, Age, Name, Birthday from VUser;");
        }

        [TestMethod]
        public void Generate_Select_Query_With_Where_From_Object_With_Virtual_Mapping()
        {
            var sql = _vUser.PocoSql().Select().Where<VUser>(u => u.UserId == 1).ToString();
            sql.Should().Be("select UserId, Age, Name, Birthday from VUser where ((UserId = @UserId));");
        }

        [TestMethod]
        public void Generate_Select_Query_With_DateTime_Where_From_Object_With_Virtual_Mapping()
        {
            DateTime fiftyYears = DateTime.Now.AddYears(-50);
            DateTime now = DateTime.Now;
            var sql = _vUser.PocoSql().Select().Where<VUser>(u => u.Birthday > fiftyYears && u.Birthday < now).ToString(); // TODO: this query is not performing correctly
            sql.Should().Be($"select UserId, Age, Name, Birthday from VUser where(((Birthday > {fiftyYears}) and(Birthday < {now})));");
        }

        [TestMethod]
        [ExpectedException(typeof(Exceptions.CantUpdateVirtualException))]
        public void Insert_Or_Update_Statements_On_Virtual_Mapping_Objects_Should_Throw_Exception()
        {
            var sql = _vUser.PocoSql().Insert().ToString();
        }
    }
}
