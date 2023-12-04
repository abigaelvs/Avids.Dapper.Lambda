using System;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Extension;

namespace Avids.Dapper.Lambda.Helper
{
    /// <inheritdoc />
    /// <summary>
    /// Trim expression tree
    /// </summary>
    internal class TrimExpression : ExpressionVisitor
    {
        private bool _isDeep;

        public static Expression Trim(Expression expression)
        {
            return new TrimExpression().Visit(expression);
        }

        /// <summary>
        /// Sub
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression Sub(Expression expression)
        {
            Type type = expression.Type;
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    if (TypeHelper.GetNonNullableType(expression.Type) == TypeHelper.GetNonNullableType(type))
                        return Expression.Constant(((ConstantExpression)expression).Value, type);
                    break;

                case ExpressionType.MemberAccess:
                    MemberExpression mExpression = expression as MemberExpression;
                    MemberExpression root = mExpression.GetRootMember();
                    if (root != null)
                    {
                        object value = mExpression.MemberToValue(root);
                        return Expression.Constant(value, type);
                    }
                    else
                    {
                        if (_isDeep)
                            return expression;

                        _isDeep = true;
                        return Expression.Equal(expression, Expression.Constant(true));
                    }

                case ExpressionType.Convert:
                    UnaryExpression u = (UnaryExpression)expression;
                    if (TypeHelper.GetNonNullableType(u.Operand.Type) == TypeHelper.GetNonNullableType(type))
                    {
                        expression = u.Operand;
                        return expression;
                    }

                    if (u.Operand.Type.IsEnum && u.Operand.NodeType == ExpressionType.MemberAccess)
                    {
                        MemberExpression mem = u.Operand as MemberExpression;
                        if (mem.Expression.NodeType == ExpressionType.Parameter)
                        {
                            return expression;
                        }
                        else
                        {
                            object value = Convert.ChangeType(mem.MemberToValue(), type);
                            return Expression.Constant(value, type);
                        }
                    }
                    break;

                case ExpressionType.Not:
                    UnaryExpression n = (UnaryExpression)expression;
                    return Expression.Equal(n.Operand, Expression.Constant(false));
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    BinaryExpression b = (BinaryExpression)expression;
                    _isDeep = true;
                    if (b.Left.NodeType != b.Right.NodeType)
                    {
                        if (b.Left.NodeType == ExpressionType.MemberAccess && b.Left.Type.Name == "Boolean")
                        {
                            if (expression.NodeType == ExpressionType.AndAlso)
                                return Expression.AndAlso(Expression.Equal(b.Left, Expression.Constant(true)), b.Right);
                            if (expression.NodeType == ExpressionType.OrElse)
                                return Expression.OrElse(Expression.Equal(b.Left, Expression.Constant(true)), b.Right);
                        }
                        if (b.Right.NodeType == ExpressionType.MemberAccess && b.Right.Type.Name == "Boolean")
                        {
                            if (expression.NodeType == ExpressionType.AndAlso)
                                return Expression.AndAlso(b.Left, Expression.Equal(b.Right, Expression.Constant(true)));
                            if (expression.NodeType == ExpressionType.OrElse)
                                return Expression.OrElse(b.Left, Expression.Equal(b.Right, Expression.Constant(true)));
                        }
                        if (b.Left.NodeType == ExpressionType.Constant)
                            return b.Right;
                        if (b.Right.NodeType == ExpressionType.Constant)
                            return b.Left;
                    }
                    break;
                default:
                    _isDeep = true;
                    return expression;
            }

            return expression;
        }

        /// <summary>
        /// Visit
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;

            exp = Sub(exp);
            return base.Visit(exp);
        }
    }
}
