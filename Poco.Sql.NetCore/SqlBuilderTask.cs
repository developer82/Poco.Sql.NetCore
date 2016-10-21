using System.Collections.Generic;
using System.Linq.Expressions;
using Poco.Sql.NetCore.Interfaces;

namespace Poco.Sql.NetCore
{
    class SqlBuilderTask
    {
        internal enum QueryTypeEnum { Select, Insert, Update, Delete, StoredProcedure, CreateTable, AlterTable, DropTable, TruncateTable, PreDefined, QueryByName, SelectIdentity, SelectScopeIdentity }

        public QueryTypeEnum QueryType { get; set; }
        public string TableName { get; set; }
        public string QueryName { get; set; }
        public List<string> WhereStrings { get; set; }
        public List<Expression> WhereExpressions { get; set; }
        public object PrimaryKeyValue { get; set; }
        public string SqlResult { get; set; }
        public string WhereCondition { get; set; }
        public IPocoSqlMapping Map { get; set; }
        public bool EndWithSemicolon { get; set; }

        public SqlBuilderTask()
        {
            WhereStrings = new List<string>();
            WhereExpressions = new List<Expression>();
            EndWithSemicolon = true;
        }
    }
}
