using System;

namespace Poco.Sql.NetCore
{
    public static class PocoSqlExtensions
    {
        public static SqlBuilder PocoSql(this Object obj)
        {
            return new SqlBuilder(obj);
        }
    }
}
