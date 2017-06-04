using System;

namespace Poco.Sql.NetCore
{
    public static class PocoSqlExtensions
    {
        public static SqlBuilder PocoSql(this Object obj)
        {
            return new SqlBuilder(obj);
        }

        public static SqlBuilder PocoSql<T>()
        {
            var obj = (object)Activator.CreateInstance<T>();
            return new SqlBuilder(obj);
        }

        public static SqlBuilder PocoSql<T>(this Object obj)
        {
            var pocoObj = (object)Activator.CreateInstance<T>();
            return new SqlBuilder(pocoObj);
        }
    }
}
