using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Avids.Dapper.Lambda.Helper;

namespace Avids.Dapper.Lambda.Extension
{
    internal static class ExpressionExtension
    {
        #region Expression Type Dictionary
        /// <summary>
        /// Expression Type Dictionary
        /// </summary>
        private static readonly Dictionary<ExpressionType, string> NodeTypeDic = new Dictionary<ExpressionType, string>
        {
            {ExpressionType.AndAlso," AND "},
            {ExpressionType.OrElse," OR "},
            {ExpressionType.Equal," = "},
            {ExpressionType.NotEqual," != "},
            {ExpressionType.LessThan," < "},
            {ExpressionType.LessThanOrEqual," <= "},
            {ExpressionType.GreaterThan," > "},
            {ExpressionType.GreaterThanOrEqual," >= "}
        };
        #endregion

        #region Get Expression TypeConversion Result
        /// <summary>
        /// Get Expression TypeConversion Result
        /// </summary>
        /// <param name="node">Binary Expression</param>
        /// <returns></returns>
        public static string GetExpressionType(this BinaryExpression node)
        {
            string nodeTypeDic = NodeTypeDic[node.NodeType];

            string nodeType = null;
            if (node.Right.NodeType == ExpressionType.Constant && ((ConstantExpression)node.Right).Value == null)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Equal:
                        nodeType = " IS ";
                        break;
                    case ExpressionType.NotEqual:
                        nodeType = " IS NOT ";
                        break;
                }
            }

            return !string.IsNullOrEmpty(nodeType) ? nodeType : nodeTypeDic;
        }
        #endregion

        #region Get the lowest member expression
        /// <summary>
        /// Get the lowest member expression
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static MemberExpression GetRootMember(this MemberExpression e)
        {
            if (e.Expression == null || e.Expression.NodeType == ExpressionType.Constant)
                return e;

            return e.Expression.NodeType == ExpressionType.MemberAccess
                ? ((MemberExpression)e.Expression).GetRootMember()
                : null;
        }
        #endregion

        #region Convert to a unary expression and evaluate
        /// <summary>
        /// Convert to a unary expression and evaluate
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static object ToConvertAndGetValue(this Expression expression)
        {
            if (expression.Type != typeof(object))
                expression = Expression.Convert(expression, typeof(object));

            Expression<Func<object>> lambdaExpression = Expression.Lambda<Func<object>>(expression);
            return lambdaExpression.Compile().Invoke();
        }
        #endregion


        /// <summary>
        /// Member to Value
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static object MemberToValue(this MemberExpression memberExpression)
        {
            MemberExpression topMember = GetRootMember(memberExpression);
            if (topMember == null)
                throw new InvalidOperationException("The conditional expression to be calculated only supports MemberExpression and ConstantExpression expression consisting of");

            return memberExpression.MemberToValue(topMember);
        }

        /// <summary>
        /// Convert Member expression to Value
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <param name="topMember"></param>
        /// <returns></returns>
        public static object MemberToValue(this MemberExpression memberExpression, MemberExpression topMember)
        {
            if (topMember.Expression == null)
            {
                //var aquire = Cache.GetOrAdd(memberExpression.ToString(), key => GetStaticProperty(memberExpression));
                Func<object, object[], object> aquire = GetStaticProperty(memberExpression);
                return aquire(null, null);
            }
            else
            {
                //var aquire = Cache.GetOrAdd(memberExpression.ToString(), key => GetInstanceProperty(memberExpression, topMember));

                Func<object, object[], object> aquire = GetInstanceProperty(memberExpression, topMember);
                return aquire((topMember.Expression as ConstantExpression).Value, null);
            }
        }

        /// <summary>
        /// Get Instance Property
        /// </summary>
        /// <param name="e"></param>
        /// <param name="topMember"></param>
        /// <returns></returns>
        private static Func<object, object[], object> GetInstanceProperty(Expression e, MemberExpression topMember)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(object), "local");
            ParameterExpression parameters = Expression.Parameter(typeof(object[]), "args");
            UnaryExpression castExpression = Expression.Convert(parameter, topMember.Member.DeclaringType);
            MemberExpression localExpression = topMember.Update(castExpression);
            Expression replaceExpression = ExpressionModifier.Replace(e, topMember, localExpression);
            replaceExpression = Expression.Convert(replaceExpression, typeof(object));
            Expression<Func<object, object[], object>> compileExpression = Expression.Lambda<Func<object, object[], object>>(replaceExpression, parameter, parameters);
            return compileExpression.Compile();
        }

        /// <summary>
        /// Get Static Property of object
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static Func<object, object[], object> GetStaticProperty(Expression e)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(object), "local");
            ParameterExpression parameters = Expression.Parameter(typeof(object[]), "args");
            UnaryExpression convertExpression = Expression.Convert(e, typeof(object));
            Expression<Func<object, object[], object>> compileExpression = Expression.Lambda<Func<object, object[], object>>(convertExpression, parameter, parameters);
            return compileExpression.Compile();
        }

        /// <summary>
        /// Get Column Attribute Name
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static string GetColumnAttributeName(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttribute<ColumnAttribute>()?.Name ?? memberInfo.Name;
        }
    }
}
