using System;

namespace Poco.Sql.NetCore.Exceptions
{
    public class NoSqlBuilderTaskFound : Exception
    {
        public override string Message
        {
            get
            {
                return "No tasks were found for sql build.";
            }
        }
    }
}
