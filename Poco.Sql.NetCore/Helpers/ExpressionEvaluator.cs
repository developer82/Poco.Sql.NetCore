using System;
using System.Linq.Expressions;

namespace Poco.Sql.NetCore.Helpers
{
    class ExpressionEvaluator
    {
        #region Eval
        internal static string Eval(object expression, bool injectValuesToQueies)
        {
            return Eval(expression, null, String.Empty, injectValuesToQueies);
        }

        internal static string Eval(object expression, object parentExpression, string statementStr, bool injectValuesToQueies)
        {
            if (expression is BinaryExpression)
                statementStr += processBinaryExpression((BinaryExpression)expression, injectValuesToQueies);
            else if (expression is MethodCallExpression)
                statementStr += processMethodCallExpression((MethodCallExpression)expression, (Expression)parentExpression, injectValuesToQueies);
            else if (expression is UnaryExpression)
                statementStr += processUnaryExpression((UnaryExpression)expression, (Expression)expression, injectValuesToQueies);
            else if (expression is MemberExpression)
                statementStr += processMemberExpression((MemberExpression)expression, parentExpression, injectValuesToQueies);
            else if (expression is ConstantExpression)
                statementStr += processConstantExpression((ConstantExpression)expression, parentExpression, injectValuesToQueies);

            return statementStr;
        }
        #endregion

        #region processBinaryExpression
        private static string processBinaryExpression(BinaryExpression binaryExpression, bool injectValuesToQueies)
        {
            string result;

            var leftExpression = binaryExpression.Left;
            var rightExpression = binaryExpression.Right;

            result = String.Format("({0}{1}{2})",
                Eval(leftExpression, binaryExpression, String.Empty, injectValuesToQueies),
                convertNodeTypeToStr(binaryExpression.NodeType),
                Eval(rightExpression, binaryExpression, String.Empty, injectValuesToQueies));

            return result;
        }
        #endregion

        #region processMethodCallExpression
        private static string processMethodCallExpression(MethodCallExpression methodCallExpression, Expression parentExpression, bool injectValuesToQueies)
        {
            string result = String.Empty;
            string constantExpressionValue = String.Empty;

            if (methodCallExpression.Arguments.Count > 0)
            {
                if (methodCallExpression.Arguments[0] is ConstantExpression)
                    constantExpressionValue = ((ConstantExpression) methodCallExpression.Arguments[0]).Value.ToString();
                else if (methodCallExpression.Arguments[0] is MemberExpression)
                    constantExpressionValue = processMemberExpression((MemberExpression) methodCallExpression.Arguments[0],
                        parentExpression, injectValuesToQueies);
            }

            var expressionStr = methodCallExpression.Object.ToString();
            expressionStr = expressionStr.Substring(expressionStr.LastIndexOf('.') + 1);

            var value = String.Empty;
            var valueWrapper = methodCallExpression.Object.Type == typeof(String) ? "'" : String.Empty;
            var entireExpStr = methodCallExpression.ToString();

            if (entireExpStr.IndexOf(".Equals") > -1)
            {
                if (!injectValuesToQueies)
                {
                    constantExpressionValue = "@" + expressionStr;
                    valueWrapper = String.Empty;
                }

                result = String.Format("{0} = {1}{2}{1}",
                    expressionStr,
                    valueWrapper,
                    constantExpressionValue);
            }
            else if (entireExpStr.IndexOf(".IndexOf") > -1)
            {
                BinaryExpression binaryParentExp = (BinaryExpression)parentExpression;
                int indexOfComparisonValue = Expression.Lambda<Func<int>>(binaryParentExp.Right).Compile().Invoke();

                if (methodCallExpression.NodeType == ExpressionType.GreaterThan && indexOfComparisonValue == -1)
                {
                    result = String.Format(" like '%{0}%'", constantExpressionValue);
                }
            }
            else
            {
                string expressionValue = getExpressionValue(methodCallExpression);
                if (String.IsNullOrEmpty(expressionValue))
                    result = expressionStr;
                else
                    result = expressionValue;
            }

            return result;
        }
        #endregion

        #region processUnaryExpression
        private static string processUnaryExpression(UnaryExpression unaryExpression, Expression parentExpression, bool injectValuesToQueies)
        {
            string result = String.Empty;

            if (unaryExpression.NodeType == ExpressionType.Not || unaryExpression.NodeType == ExpressionType.NotEqual)
                result = "not ";

            result += Eval(unaryExpression.Operand, parentExpression, String.Empty, injectValuesToQueies);
            return result;
        }
        #endregion

        #region processMemberExpression
        private static string processMemberExpression(MemberExpression memberExpression, object parentExpression, bool injectValuesToQueies)
        {
            string result = String.Empty;
            if (memberExpression.Expression is ConstantExpression)
            {
                if (!injectValuesToQueies && parentExpression is BinaryExpression)
                    return getParameterziedValue(memberExpression, (BinaryExpression)parentExpression);

                return getExpressionValue(memberExpression);
            }
            else
            {
                result = memberExpression.ToString();
                result = result.Substring(result.LastIndexOf('.') + 1);
            }
            return result;
        }
        #endregion

        #region processConstantExpression
        private static string processConstantExpression(ConstantExpression expression, object parentExpression, bool injectValuesToQueies)
        {
            if (!injectValuesToQueies && parentExpression is BinaryExpression)
                return getParameterziedValue(expression, (BinaryExpression)parentExpression);

            return expression.ToString();
        }
        #endregion

        #region getExpressionValue
        private static string getExpressionValue(Expression expression)
        {
            try
            {
                Expression conversion = Expression.Convert(expression, typeof(object));
                Func<object> func = Expression.Lambda<Func<object>>(conversion).Compile();

                object value = func.Invoke();
                if (value == null)
                    return string.Empty;

                return value.ToString();
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }
        #endregion

        #region getParameterziedValue
        private static string getParameterziedValue(Expression originalExpression, BinaryExpression parentExpression)
        {
            string fieldName;
            if (parentExpression.Left == originalExpression)
                fieldName = Eval(parentExpression.Right, false);
            else
                fieldName = Eval(parentExpression.Left, false);

            return "@" + fieldName;
        }
        #endregion

        #region convertNodeTypeToStr
        private static string convertNodeTypeToStr(ExpressionType nodeType)
        {
            string result;
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    result = " = ";
                    break;
                case ExpressionType.Not:
                case ExpressionType.NotEqual:
                    result = " not ";
                    break;
                case ExpressionType.GreaterThan:
                    result = " > ";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    result = " >= ";
                    break;
                case ExpressionType.LessThan:
                    result = " < ";
                    break;
                case ExpressionType.LessThanOrEqual:
                    result = " <= ";
                    break;
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    result = " and ";
                    break;
                case ExpressionType.OrElse:
                case ExpressionType.Or:
                    result = " or ";
                    break;
                default:
                    result = String.Empty;
                    break;
            }

            return result;
        }
        #endregion
    }
}
