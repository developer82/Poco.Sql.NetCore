using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Poco.Sql.NetCore
{
    public class ValuesObject
    {
        private static readonly SqlDateTime _smallDateTimeMinValue = new SqlDateTime(new DateTime(1900, 01, 01, 00, 00, 00));

        public static object Create(params object[] args)
        {
            dynamic obj = new ExpandoObject();
            
            List<string> sqlParams = GetPramsFromSqlString(args);
            int startPos = sqlParams == null ? 0 : 1; // if we have sqlParams that means that the first agrument is an sql string

            // loop over all objects that were sent for merging
            for (int i = startPos; i < args.Length; i++)
            {
                // get an object and make sure it's valid
                object currentObj = args[i];
                if (currentObj == null) continue;

                IDictionary<string, object> objDic = (IDictionary<string, object>) obj;
                
                // loop over all elements of current object
                IEnumerable<PropertyInfo> propertyInfos = currentObj.GetType().GetRuntimeProperties();
                foreach (
                    PropertyInfo propertyInfo in
                        propertyInfos.Where(
                            p =>
                                p.PropertyType.GetTypeInfo().IsEnum ||
                                (p.PropertyType.FullName.StartsWith("System") &&
                                !p.PropertyType.FullName.StartsWith("System.Collections"))))
                    // only loop on objects that are not custom class
                {
                    if (objDic.ContainsKey(propertyInfo.Name))
                        continue; // same key can't be added twice (first key found will be used)

                    if (sqlParams == null || sqlParams.Contains(propertyInfo.Name))
                    {
                        var val = propertyInfo.GetValue(currentObj);
                        if (propertyInfo.PropertyType == typeof (DateTime) && (DateTime) val == DateTime.MinValue)
                            val = (DateTime) System.Data.SqlTypes.SqlDateTime.MinValue;

                        objDic.Add(propertyInfo.Name, val);
                    }
                }
            }

            return (object)obj;
        }

        private static object getDefaultValue(Type t)
        {
            if (t == typeof(DateTime))
                return DateTime.MinValue;
            else if (t.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        private static List<string> GetPramsFromSqlString(object[] args)
        {
            List<string> sqlParams = null;
            if (args.Length > 1 && args[0] is String)
            {
                sqlParams = new List<string>();

                string sql = args[0].ToString();
                Regex regex = new Regex(@"(?<=@)([\w\-]+)");
                var matches = regex.Matches(sql);

                for (int i = 0; i < matches.Count; i++)
                {
                    var val = matches[i].Value;
                    if (!sqlParams.Contains(val))
                        sqlParams.Add(matches[i].Value);
                }
            }
            return sqlParams;
        }
    }
}