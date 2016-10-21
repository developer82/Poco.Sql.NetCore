using System;
using System.Collections.Generic;
using Poco.Sql.NetCore.Interfaces;

namespace Poco.Sql.NetCore
{
    public class Configuration
    {
        private static Object _lock = new Object();
        private static bool _initialized;
        private static Dictionary<string, object> _mappings = new Dictionary<string, object>();

        internal static bool IsPluralizeTableNames { get; private set; }
        internal static bool FullGraphSelecttion { get; private set; }
        internal static bool ValuesInQueies { get; private set; }
        internal static bool CachingDisabled { get; private set; }
        internal static bool Comment { get; private set; }
        internal static string StoredProceduresPrefix { get; private set; }
        internal static Dictionary<string, object> Mappings = new Dictionary<string, object>();

        public static void Initialize(Action<Configuration> config)
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;
                config.Invoke(new Configuration());
                _initialized = true;
            }
        }

        public void PluralizeTableNames()
        {
            IsPluralizeTableNames = true;
        }

        public void SelectFullGraph()
        {
            FullGraphSelecttion = true;
        }

        public void InjectValuesInQueies()
        {
            ValuesInQueies = true;
        }

        public void DisableCache()
        {
            CachingDisabled = true;
        }

        public void ShowComments()
        {
            Comment = true;
        }

        public void SetStoreProcedurePrefix(string prefix)
        {
            StoredProceduresPrefix = prefix;
        }

        public void AddMap<T>(PocoSqlMapping<T> mapping)
        {
            _mappings.Add(typeof(T).FullName, mapping);
        }

        internal static bool HasMap(string key)
        {
            return _mappings.ContainsKey(key);
        }

        internal static IPocoSqlMapping GetMap(string key)
        {
            return (IPocoSqlMapping)_mappings[key];
        }
    }
}
