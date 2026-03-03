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

                        Type newValue = value == null ? type : value.GetType();

                        //return Expression.Constant(value, type);
                        return Expression.Constant(value, newValue);
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

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            // Kalau operator == atau != dan tipe kiri-kanan beda tapi secara dasar sama (nullable vs non-nullable)
            if ((node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual)
                && TypeHelper.GetNonNullableType(left.Type) == TypeHelper.GetNonNullableType(right.Type)
                && left.Type != right.Type)
            {
                // Naikkan ke nullable yang lebih luas
                var targetType = GetWiderNullableType(left.Type, right.Type);

                left = left.Type != targetType ? Expression.Convert(left, targetType) : left;
                right = right.Type != targetType ? Expression.Convert(right, targetType) : right;

                return Expression.MakeBinary(node.NodeType, left, right);
            }

            return node.Update(left, node.Conversion, right);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Console.WriteLine("=== VISIT METHOD START ===");
            Console.WriteLine(node.Method.Name == "Equals" && node.Arguments.Count == 1);
            if (node.Method.Name == "Equals" && node.Arguments.Count == 1)
            {
                Expression instance = Visit(node.Object);
                Console.WriteLine("=== AFTER VISIT INSTANCE ===");
                Expression argument = Visit(node.Arguments[0]);

                Console.WriteLine("=== equal");
                Console.WriteLine(instance.Type.Name);
                Console.WriteLine(argument.Type.Name);

                if (instance.Type != argument.Type)
                {
                    Type targetType = GetWiderNullableType(instance.Type, argument.Type);

                    Console.WriteLine("=== VISIT METHOD CALL ==");
                    Console.WriteLine(targetType);

                    instance = Expression.Convert(instance, targetType);
                    argument = Expression.Convert(argument, targetType);
                }

                return Expression.Equal(instance, argument);
            }
            return base.VisitMethodCall(node);
        }

        private Type GetWiderNullableType(Type a, Type b)
        {
            Type nonNullA = Nullable.GetUnderlyingType(a) ?? a;
            Type nonNullB = Nullable.GetUnderlyingType(b) ?? b;

            if (nonNullA != nonNullB)
                throw new InvalidOperationException($"Cannot compare {a} and {b}");

            return typeof(Nullable<>).MakeGenericType(nonNullA);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            Console.WriteLine("=== VISIT UNARY ===");
            Console.WriteLine(node.NodeType == ExpressionType.Convert);

            if (node.NodeType == ExpressionType.Not && node.Operand is MethodCallExpression callExpr)
            {
                if (callExpr.Method.Name == "Equals" && callExpr.Arguments.Count == 1)
                {
                    Console.WriteLine("=== DETECTED !x.Equals(y) ===");
                    var left = Visit(callExpr.Object);
                    var right = Visit(callExpr.Arguments[0]);

                    return Expression.NotEqual(left, right);
                }
            }

            // Tangani Convert(x) -> x jika konversi ke nullable yang valid
            if (node.NodeType == ExpressionType.Convert)
            {
                var operand = Visit(node.Operand);
                Type operandType = operand.Type;
                Type targetType = node.Type;

                Console.WriteLine("=== VISIT UNARY INSIDE ===");
                Console.WriteLine(operandType.Name);
                Console.WriteLine(targetType.Name);
                Console.WriteLine(node);

                // Jika sama-sama tipe dasarnya dan salah satu nullable, boleh kita kembalikan operand-nya langsung
                if (TypeHelper.GetNonNullableType(operandType) == TypeHelper.GetNonNullableType(targetType) && targetType != typeof(object))
                {
                    return operand; // buang Convert
                }

                // Jika operand == long dan target == Nullable<long>, kita tetap butuh Convert
                return Expression.Convert(operand, operandType);
            }

            return base.VisitUnary(node);
        }
    }
}
