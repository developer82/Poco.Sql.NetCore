namespace Poco.Sql.NetCore
{
    public class PocoSqlStoredProcedureMapping
    {
        public string SelectQuery { get; private set; }
        public string InsertQuery { get; private set; }
        public string UpdateQuery { get; private set; }
        public string DeleteQuery { get; private set; }

        public void Select(string spName)
        {
            SelectQuery = spName;
        }

        public void Insert(string spName)
        {
            InsertQuery = spName;
        }

        public void Update(string spName)
        {
            UpdateQuery = spName;
        }

        public void Delete(string spName)
        {
            DeleteQuery = spName;
        }
    }
}
