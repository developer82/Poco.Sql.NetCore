namespace Poco.Sql.NetCore.Test.Models
{
    public class SuperUser : User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Permissions { get; set; }
    }
}
