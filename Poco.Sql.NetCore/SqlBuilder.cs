using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Humanizer;
using Poco.Sql.NetCore.Exceptions;
using Poco.Sql.NetCore.Helpers;
using Poco.Sql.NetCore.Interfaces;

namespace Poco.Sql.NetCore
{
    /// <summary>
    /// This class is responsible for building the SQL statements
    /// </summary>
    public class SqlBuilder
    {
        private static Dictionary<string, string> _cachedQueries = new Dictionary<string, string>(); //TODO: implement caching
        
        private List<SqlBuilderTask> _tasks;

        private object _obj;

        private SqlBuilderTask currentTask
        {
            get { return _tasks[_tasks.Count - 1]; }
        }

        public string Result
        {
            get
            {
                return getSqlStatement();
            }
        }

        public SqlBuilder(object obj)
        {
            _obj = obj;
            _tasks = new List<SqlBuilderTask>();
        }

        private void createTask(SqlBuilderTask task)
        {
            _tasks.Add(task);
        }
        
        private void createTask(SqlBuilderTask.QueryTypeEnum queryType)
        {
            var task = new SqlBuilderTask()
            {
                QueryType = queryType
            };
            createTask(task);
        }

        public SqlBuilder NamedQuery(string name)
        {
            createTask(new SqlBuilderTask()
                {
                    QueryType = SqlBuilderTask.QueryTypeEnum.QueryByName,
                    QueryName = name
                });
            
            //var map = getMapping(_obj);
            //if (map != null)
            //    _sql = map.GetQueryByName(name);
            return this;
        }

        #region Select
        public SqlBuilder Select()
        {
            createTask(SqlBuilderTask.QueryTypeEnum.Select);
            return this;
        }

        public SqlBuilder Select(long id)
        {
            createTask(new SqlBuilderTask()
                {
                    QueryType = SqlBuilderTask.QueryTypeEnum.Select,
                    PrimaryKeyValue = id
                });
            //createTask(SqlBuilderTask.QueryTypeEnum.Select, primaryKey + " = " + id);
            //string primaryKey = getPrimaryKey(_obj);
            //return buildSqlStatement(_obj, null, null, null, null).Where(primaryKey + " = " + id);
            return this;
        }
        
        public SqlBuilder Select<T>(PocoSqlMapping<T> map)
        {
            createTask(new SqlBuilderTask()
                {
                    QueryType = SqlBuilderTask.QueryTypeEnum.Select,
                    Map = map
                });

            return this;
        }

        public SqlBuilder Select(string tableName)
        {
            createTask(new SqlBuilderTask()
                {
                    QueryType = SqlBuilderTask.QueryTypeEnum.Select,
                    TableName = tableName
                });

            return this;
        }
        #endregion

        public SqlBuilder SelectScopeIdentity()
        {
            createTask(SqlBuilderTask.QueryTypeEnum.SelectScopeIdentity);
            return this;
        }

        public SqlBuilder SelectIdentity()
        {
            createTask(SqlBuilderTask.QueryTypeEnum.SelectIdentity);
            return this;
        }

        public SqlBuilder FullGraph()
        {
            throw new NotImplementedException();

            //PropertyInfo[] propertyInfos = _obj.GetType().GetProperties();

            //StringBuilder allFields = new StringBuilder();
            //foreach (PropertyInfo propertyInfo in propertyInfos.Where(p => p.PropertyType.FullName.StartsWith("System.Collections")))
            //{
            //    var collectionObject = propertyInfo.GetValue(_obj);
            //    Type collectionType = collectionObject.GetType().GetGenericArguments()[0];
            //    object obj = Activator.CreateInstance(collectionType);

            //    string key = obj.GetType().FullName;
            //    if (Configuration.HasMap(key))
            //    {
            //        var map = Configuration.GetMap(key);;
            //        var relationship = map.GetRelationship(_obj.GetType().FullName);

            //        string currentKey = _obj.GetType().FullName;
            //        var currentMap = Configuration.GetMap(key); ;

            //        var val = _obj.GetType().GetProperty(currentMap.GetPrimaryKey()).GetValue(_obj);
            //        string whereConditionStr = relationship.GetForeignKey() + " = " + val;

            //        SqlBuilder qb = new SqlBuilder(obj);
            //        string sql = qb.Select().Where(whereConditionStr).ToString();
            //        _graphSql += Environment.NewLine + sql;
            //    }
            //}

            //return this;
        }

        public SqlBuilder Update()
        {
            createTask(SqlBuilderTask.QueryTypeEnum.Update);
            return this;
        }

        public SqlBuilder Insert()
        {
            createTask(SqlBuilderTask.QueryTypeEnum.Insert);
            return this;
        }

        public SqlBuilder Delete()
        {
            createTask(SqlBuilderTask.QueryTypeEnum.Delete);
            return this;
        }

        //private SqlBuilder buildSqlStatement(object obj, SqlBuilderTask.QueryTypeEnum queryType, string tableName, bool? fullGraph, bool? pluralizeTableNames, IPocoSqlMapping map)
        private SqlBuilder buildSqlStatement(object obj, SqlBuilderTask task)
        {
            if (task.QueryType == SqlBuilderTask.QueryTypeEnum.SelectScopeIdentity)
            {
                task.SqlResult = "select scope_identity()";
                return this;
            }
            else if (task.QueryType == SqlBuilderTask.QueryTypeEnum.SelectIdentity)
            {
                task.SqlResult = "select @@identity";
                return this;
            }

            IPocoSqlMapping map = null;
            string tableName = null;
            
            if (Configuration.Comment)
                task.SqlResult = getComment(obj);

            if (task.Map == null)
                map = getMapping(obj);

            if (map != null)
            {
                //
                // Custom query
                //
                var customQuery = getCustomQuery(task.QueryType, map);
                if (!String.IsNullOrEmpty(customQuery))
                {
                    task.SqlResult += customQuery;
                    if (task.SqlResult.EndsWith(";")) currentTask.EndWithSemicolon = false;
                    return this; // if it's a custom query no other thing needs (the user knew in advanced what he wanted) to be done and the result is returned
                }

                //
                // Stored Procedures mappings
                //
                var spMappings = map.GetStoredProceduresMappings();
                if (spMappings != null)
                {
                    PocoSqlStoredProcedureMap spMap = getStoredProcedureMap(spMappings, task.QueryType);
                    if (spMap != null)
                    {
                        string spName = getStoredProcedureName(spMap, obj, task.QueryType);
                        task.SqlResult += String.Format("{0}{1}{2}",
                            spMap.Execution ? "exec " : String.Empty,
                            spName,
                            spMap.Parameters ?  getQueryFields(task.QueryType, map) : String.Empty);
                        if (!spMap.Execution && !spMap.Parameters) currentTask.EndWithSemicolon = false;
                        return this;
                    }
                }

                if (task.QueryType != SqlBuilderTask.QueryTypeEnum.Select && map.GetIsVirtual())
                    throw new CantUpdateVirtualException();

                if (String.IsNullOrEmpty(task.TableName))
                    tableName = map.GetTableName();
            }

            if (String.IsNullOrEmpty(tableName))
                tableName = obj.GetType().Name;

            if ((map == null || (map != null && String.IsNullOrEmpty(map.GetTableName()))) && Configuration.IsPluralizeTableNames)
            {
                tableName = tableName.Pluralize();
            }

            switch (task.QueryType)
            {
                case SqlBuilderTask.QueryTypeEnum.Select:
                    task.SqlResult += String.Format("select {0} from {1}", getQueryFields(SqlBuilderTask.QueryTypeEnum.Select, map), tableName);
                    break;
                case SqlBuilderTask.QueryTypeEnum.Insert:
                    task.SqlResult += String.Format("insert into {0} {1}", tableName, getQueryFields(SqlBuilderTask.QueryTypeEnum.Insert, map));
                    break;
                case SqlBuilderTask.QueryTypeEnum.Update:
                    task.SqlResult += String.Format("update {0} set {1}", tableName, getQueryFields(SqlBuilderTask.QueryTypeEnum.Update, map));
                    break;
                case SqlBuilderTask.QueryTypeEnum.Delete:
                    task.SqlResult += String.Format("delete from {0}", tableName);
                    string primaryKey = getPrimaryKey(_obj);
                    string deleteWhereValue = "@" + primaryKey;
                    if (Configuration.ValuesInQueies)
                        deleteWhereValue = getPropertyValueAsSql(_obj, primaryKey);
                    this.Where(String.Format("{0} = {1}", primaryKey, deleteWhereValue));
                    break;
                case SqlBuilderTask.QueryTypeEnum.StoredProcedure:
                    var queryFields = String.Empty;
                    var execCmd = String.Empty;
                    task.SqlResult += String.Format("exec {0} {1}", tableName, getQueryFields(SqlBuilderTask.QueryTypeEnum.StoredProcedure, map));
                    break;
            }

            return this;
        }

        private PocoSqlStoredProcedureMap getStoredProcedureMap(PocoSqlStoredProceduresMapping spMappings, SqlBuilderTask.QueryTypeEnum queryType)
        {
            switch (queryType)
            {
                case SqlBuilderTask.QueryTypeEnum.Select:
                    return spMappings.SelectMap;
                case SqlBuilderTask.QueryTypeEnum.Insert:
                    return spMappings.InsertMap;
                case SqlBuilderTask.QueryTypeEnum.Update:
                    return spMappings.UpdateMap;
                case SqlBuilderTask.QueryTypeEnum.Delete:
                    return spMappings.DeleteMap;
                default:
                    return null;
            }
        }

        private string getComment(object obj)
        {
            return String.Format(
                "/*{0}Poco.Sql{0}Time: {1}{0}Object: {2}{0}*/{0}",
                Environment.NewLine,
                DateTime.Now,
                obj.GetType().FullName
            );
        }

        private string getCustomQuery(SqlBuilderTask.QueryTypeEnum queryType, IPocoSqlMapping map)
        {
            string customQuery = null;
            var customMappings = map.GetCustomMappings();
            if (customMappings != null)
            {
                switch (queryType)
                {
                    case SqlBuilderTask.QueryTypeEnum.Select:
                        customQuery = customMappings.SelectQuery;
                        break;
                    case SqlBuilderTask.QueryTypeEnum.Insert:
                        customQuery = customMappings.InsertQuery;
                        break;
                    case SqlBuilderTask.QueryTypeEnum.Update:
                        customQuery = customMappings.UpdateQuery;
                        break;
                    case SqlBuilderTask.QueryTypeEnum.Delete:
                        customQuery = customMappings.DeleteQuery;
                        break;
                }
            }
            return customQuery;
        }

        private string getQueryFields(SqlBuilderTask.QueryTypeEnum queryType, IPocoSqlMapping map)
        {
            IEnumerable<PropertyInfo> propertyInfos = _obj.GetType().GetRuntimeProperties();

            string primaryKey = null;

            if (queryType == SqlBuilderTask.QueryTypeEnum.Update || queryType == SqlBuilderTask.QueryTypeEnum.Insert)
                primaryKey = getPrimaryKey(_obj);

            StringBuilder allFields = new StringBuilder();
            StringBuilder insertValues = new StringBuilder();
            
            foreach (PropertyInfo propertyInfo in propertyInfos.Where(p => p.PropertyType.FullName.StartsWith("System") && !p.PropertyType.FullName.StartsWith("System.Collections"))) // only loop on objects that are not custom class
            {
                string
                    dbSelect = null,
                    fieldName = null,
                    dbColumnName = null;

                if (map != null)
                {
                    PropertyMap propertyMap = map.GetMapping(propertyInfo.Name);
                    if (propertyMap != null)
                    {
                        if (!propertyMap.Ignored)
                        {
                            if (!propertyMap.ColumnName.Equals(propertyInfo.Name))
                            {
                                if (queryType == SqlBuilderTask.QueryTypeEnum.Select)
                                    dbSelect = String.Format("{0} as {1}",
                                        propertyMap.ColumnName,
                                        propertyInfo.Name);
                                fieldName = propertyInfo.Name;
                                dbColumnName = propertyMap.ColumnName;
                            }
                        }
                        else
                        {
                            dbSelect = String.Empty;
                        }
                    }
                }

                if (String.IsNullOrEmpty(dbSelect))
                    dbSelect = fieldName = dbColumnName = propertyInfo.Name;

                if (fieldName == primaryKey)
                {
                    if (queryType == SqlBuilderTask.QueryTypeEnum.Update)
                    {
                        var property = _obj.GetType().GetRuntimeProperty(dbColumnName);
                        string val = "@" + primaryKey;
                        if (Configuration.ValuesInQueies)
                            property.GetValue(_obj).ToString();
                        this.Where(primaryKey + " = " + val);
                    }

                    if (queryType == SqlBuilderTask.QueryTypeEnum.Update || (queryType == SqlBuilderTask.QueryTypeEnum.Insert && map.GetPrimaryAutoGenerated()))
                        continue;
                }
                
                switch(queryType)
                {
                    case SqlBuilderTask.QueryTypeEnum.Select:
                        allFields.Append((allFields.Length > 0 && !String.IsNullOrEmpty(fieldName) ? ", " : String.Empty) + dbSelect);
                        break;

                    case SqlBuilderTask.QueryTypeEnum.Insert:
                        if (!String.IsNullOrEmpty(fieldName))
                        {
                            string insertVal;
                            if (Configuration.ValuesInQueies)
                                insertVal = getPropertyValueAsSql(_obj, fieldName);
                            else
                                insertVal = "@" + fieldName;

                            allFields.Append((allFields.Length > 0 ? ", " : String.Empty) + dbSelect);
                            insertValues.Append((insertValues.Length > 0 ? ", " : String.Empty) + insertVal);
                        }
                        break;

                    case SqlBuilderTask.QueryTypeEnum.Update:
                    case SqlBuilderTask.QueryTypeEnum.StoredProcedure:
                        string updateVal;
                        if (Configuration.ValuesInQueies)
                            updateVal = getPropertyValueAsSql(_obj, fieldName, true);
                        else
                            updateVal = "@" + fieldName;
                        
                        allFields.Append((allFields.Length > 0 && !String.IsNullOrEmpty(fieldName) ? ", " : String.Empty) + dbColumnName + " = " + updateVal);
                        break;

                    case SqlBuilderTask.QueryTypeEnum.Delete:

                        break;
                }
            }

            if (queryType == SqlBuilderTask.QueryTypeEnum.Insert)
                return String.Format("({0}) values({1})", allFields, insertValues);
            else
                return allFields.ToString();
        }

        private string getPropertyValueAsSql(object obj, string propertyName)
        {
            return getPropertyValueAsSql(obj, propertyName, false);
        }

        private string getPropertyValueAsSql(object obj, string propertyName, bool cast)
        {
            PropertyInfo property = obj.GetType().GetRuntimeProperty(propertyName);
            string result = property.GetValue(_obj).ToString();

            if ((property.PropertyType == typeof(string) || !cast) && property.PropertyType != typeof(int))
                result = "'" + result + "'";
            else if (property.PropertyType == typeof(DateTime))
                result = "cast('" + result + "' as datetime)";

            return result;
        }

        private string getStoredProcedureName(PocoSqlStoredProcedureMap spMap, object obj, SqlBuilderTask.QueryTypeEnum queryType)
        {
            if (spMap == null) return String.Empty;
            if (String.IsNullOrEmpty(spMap.Name))
                return String.Format("{0}{1}_{2}", Configuration.StoredProceduresPrefix, obj.GetType().Name, queryType.ToString());

            return spMap.Name;
        }

        private IPocoSqlMapping getMapping(object obj)
        {
            IPocoSqlMapping map = null;
            string key = obj.GetType().FullName;
            if (Configuration.HasMap(key))
                map = Configuration.GetMap(key);
            return map;
        }

        #region Join
        public SqlBuilder Join<T>(Expression<Func<T, bool>> on)
        {
            return Join<T>(typeof(T).Name, on);
        }

        public SqlBuilder Join<T>(string tableName, Expression<Func<T, bool>> on)
        {
            throw new NotImplementedException();
            return this;
        }
        #endregion

        #region Where
        public SqlBuilder Where(string condition)
        {
            //TODO: make sure update statements can't have a a where clause
            currentTask.WhereStrings.Add(condition);
            return this;
        }

        public SqlBuilder Where<TSource>(Expression<Func<TSource, bool>> condition)
        {
            //TODO: make sure update statements can't have a a where clause
            currentTask.WhereExpressions.Add(condition.Body);
            //_where += " where " + ExpressionEvaluator.Eval(condition.Body, Configuration.ValuesInQueies);
            return this;
        }
        #endregion

        private string getPrimaryKey(object obj)
        {
            var map = getMapping(_obj);
            if (map != null && !String.IsNullOrEmpty(map.GetPrimaryKey()))
                return map.GetPrimaryKey(); // return a mapped primary key

            // check by convension
            PropertyInfo property = null;
            property = obj.GetType().GetRuntimeProperties().Where(p => p.Name.ToLower() == "id").FirstOrDefault();
            if (property != null)
                return property.Name;

            property = obj.GetType().GetRuntimeProperties().Where(p => p.Name.ToLower() == obj.GetType().Name.ToLower() + "id").FirstOrDefault();
            if (property != null)
                return property.Name;

            return null;
        }

        private void buildWhereCondition(object _obj, SqlBuilderTask task)
        {
            if (task.WhereStrings.Count > 0)
            {
                foreach (var whereString in task.WhereStrings)
                {
                    task.WhereCondition += String.IsNullOrEmpty(task.WhereCondition)
                        ? " where (" + whereString + ")"
                        : " and (" + whereString + ")";
                }
            }

            if (task.WhereExpressions.Count == 0) return;
            foreach (var whereExpression in task.WhereExpressions)
            {
                var expressionStr = ExpressionEvaluator.Eval(whereExpression, Configuration.ValuesInQueies);
                task.WhereCondition += String.IsNullOrEmpty(task.WhereCondition)
                    ? " where (" + expressionStr + ")"
                    : " and (" + expressionStr + ")";
            }
        }

        public override string ToString()
        {
            return getSqlStatement();
        }

        public string ToSQL()
        {
            return getSqlStatement();
        }

        private string getSqlStatement()
        {
            if (_tasks.Count == 0) // there must be at least one task
                throw new NoSqlBuilderTaskFound();

            StringBuilder result = new StringBuilder();
            foreach (var task in _tasks)
            {
                buildSqlStatement(_obj, task);
                buildWhereCondition(_obj, task);

                var sql = String.Format("{0}{1}{2}",
                    task.SqlResult,
                    task.WhereCondition,
                    task.EndWithSemicolon ? ";" : String.Empty);

                result.Append(sql);
            }

            return result.ToString();
        }
    }
}
