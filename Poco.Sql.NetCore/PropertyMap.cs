using System;

namespace Poco.Sql.NetCore
{
    public class PropertyMap
    {
        public string PropertyName { get; private set; }
        public string ColumnName { get; private set; }
        public bool Required { get; private set; }
        public bool Ignored { get; private set; }
        public int MaxLength { get; private set; }
        public Type FieldType { get; private set; }

        public PropertyMap(string propertyName)
        {
            PropertyName = propertyName;
            ColumnName = propertyName;
        }

        public PropertyMap HasColumnName(string columnName)
        {
            ColumnName = columnName;
            return this;
        }

        public PropertyMap Ignore()
        {
            Ignored = true;
            return this;
        }

        public PropertyMap IsRequired() //TODO: validate this property when building the query
        {
            return IsRequired(true);
        }

        public PropertyMap IsRequired(bool required)
        {
            Required = required;
            return this;
        }

        public PropertyMap HasMaxLength(int maxLength) //TODO: validate this property when building the query
        {
            MaxLength = maxLength;
            return this;
        }
    }
}
