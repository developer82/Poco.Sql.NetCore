namespace Poco.Sql.NetCore
{
    public class PocoSqlStoredProcedureMap
    {
        public string Name { get; private set; } // The name of the stored procedure
        public bool Parameters { get; private set; } // By default the generated sql statement will not include the object parameters when mapped to stored procedure (most micro-orm frameworks does that and only require SP name)
        public bool Execution { get; private set; } // By default the generated sql statement will not include the 'exec' command (most micro-orm frameworks does that and only require SP name)

        public PocoSqlStoredProcedureMap HasName(string spName)
        {
            Name = spName;
            return this;
        }

        public PocoSqlStoredProcedureMap IncludeParameters()
        {
            Parameters = true;
            return this;
        }

        public PocoSqlStoredProcedureMap IncludeExecution()
        {
            Execution = true;
            return this;
        }
    }
}
