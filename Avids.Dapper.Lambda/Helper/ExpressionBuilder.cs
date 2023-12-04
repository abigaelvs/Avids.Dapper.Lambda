using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Avids.Dapper.Lambda.Helper
{
    public static class ExpressionBuilder
    {
        /// <summary>
        /// Default True condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Init<T>()
        {
            return expression => true;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
            Func<Expression, Expression, Expression> merge
        )
        {
            Dictionary<ParameterExpression, ParameterExpression> map = 
                first.Parameters
                .Select((oldParam, index) => new { 
                    oldParam, newParam = second.Parameters[index] })
                .ToDictionary(p => p.newParam, p => p.oldParam);

            Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }

    internal class ParameterRebinder : ExpressionVisitor
    {
        readonly Dictionary<ParameterExpression, ParameterExpression> _parameterMap;

        ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _parameterMap = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// Replace Parameters
        /// </summary>
        /// <param name="map"></param>
        /// <param name="newParameters"></param>
        /// <returns></returns>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map,
            Expression newParameters)
        {
            return new ParameterRebinder(map).Visit(newParameters);
        }

        /// <summary>
        /// Visit Parameter Expression
        /// </summary>
        /// <param name="newParameters"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression newParameters)
        {
            if (_parameterMap.TryGetValue(newParameters, out ParameterExpression replacement))
            {
                newParameters = replacement;
            }

            return base.VisitParameter(newParameters);
        }
    }
}
