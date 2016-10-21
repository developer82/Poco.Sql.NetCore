using System;

namespace Poco.Sql.NetCore
{
    public class StatementsCreator
    {
        public static SqlBuilder Create<T>()
        {
            var dummy = (T) Activator.CreateInstance(typeof (T));
            return new SqlBuilder(dummy);
        }
    }
}
