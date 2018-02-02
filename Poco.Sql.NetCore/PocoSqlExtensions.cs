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

        public static SqlBuilder SqlBuilder(this Object obj)
        {
            return new SqlBuilder(obj);
        }

        public static SqlBuilder SqlBuilder<T>()
        {
            var obj = (object)Activator.CreateInstance<T>();
            return new SqlBuilder(obj);
        }

        public static SqlBuilder SqlBuilder<T>(this Object obj)
        {
            var pocoObj = (object)Activator.CreateInstance<T>();
            return new SqlBuilder(pocoObj);
        }

        public static SqlBuilder Select(this Object obj)
        {
            return (new SqlBuilder(obj)).Select();
        }

        public static SqlBuilder Insert(this Object obj)
        {
            return (new SqlBuilder(obj)).Insert();
        }

        public static SqlBuilder Update(this Object obj)
        {
            return (new SqlBuilder(obj)).Update();
        }

        public static SqlBuilder Delete(this Object obj)
        {
            return (new SqlBuilder(obj)).Delete();
        }
    }
}
