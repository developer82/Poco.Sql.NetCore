using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Poco.Sql.NetCore.Interfaces;

namespace Poco.Sql.NetCore
{
    public class RelationshipMap<T, TTarget> : IRelationshipMap
    {
        private string _currentObject;
        private string _foreignKey;
        public Type RelatedObject { get; set; }

        public RelationshipMap<T, TTarget> WithMany(Expression<Func<TTarget, ICollection<T>>> navigationPropertyExpression)
        {
            _currentObject = navigationPropertyExpression.Body.ToString();
            _currentObject = _currentObject.Substring(_currentObject.LastIndexOf('.') + 1);
            return this;
        }

        public RelationshipMap<T, TTarget> HasForeignKey<TKey>(Expression<Func<T, TKey>> foreignKeyExpression)
        {
            _foreignKey = foreignKeyExpression.Body.ToString();
            _foreignKey = _foreignKey.Substring(_foreignKey.LastIndexOf('.') + 1);
            return this;
        }


        public string GetForeignKey()
        {
            return _foreignKey;
        }


        public Type GetRelatedObjectType()
        {
            return RelatedObject;
        }
    }
}
