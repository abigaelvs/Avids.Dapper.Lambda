using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Extension;
using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Exception;
using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Expressions
{
    /// <summary>
    /// Where Expression Builder
    /// </summary>
    public class WhereExpression : SqlCmdExpression
    {
        /// <summary>
        /// SQL string result
        /// </summary>
        public override string SqlCmd => _sqlCmd.Length > 0 ? $"WHERE {_sqlCmd}" : string.Empty;
        private string _tempFieldName;
        private string TempFieldName
        {
            get => _prefix + _tempFieldName + ParameterCount;
            set => _tempFieldName = value;
        }
        private string ParamName => _parameterPrefix + TempFieldName;
        private int ParameterCount { get; set; }
        private readonly bool _withTableName;

        protected WhereExpression(string prefix, ProviderOption providerOption,
            bool withTableName = false) : base(prefix, providerOption)
        {
            _withTableName = withTableName;
        }

        public WhereExpression(Queue<Where> whereExpressions , string prefix, ProviderOption providerOption,
            bool withTableName = false) : this(prefix, providerOption, withTableName)
        {
            while (whereExpressions.Count > 0)
            {
                Where curr = whereExpressions.Dequeue();
                if (curr.WhereType != null) _sqlCmd.Append($" {curr.WhereType} ");
                Visit(TrimExpression.Trim(curr.WhereExpression));
            }
        }

        /// <summary>
        /// Visit Member for Where Expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            string tableName = node.Member.DeclaringType.GetTableAttributeName();
            string table = _withTableName ? $"{_providerOption.CombineFieldName(tableName)}." : "";
            string field = _providerOption.CombineFieldName(node.Member.GetColumnAttributeName());
            
            _sqlCmd.Append($"{table}{field}");
            TempFieldName = _providerOption.CombineFieldName(node.Member.GetColumnAttributeName(), true);
            ParameterCount++;
            return node;
        }

        /// <summary>
        /// Visit Binary for Where Expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            // Not Like
            if (node.Left.NodeType == ExpressionType.Call && node.Right.NodeType == ExpressionType.Constant)
            {
                MethodCallExpression call = node.Left as MethodCallExpression;
                if (ExpressionExtension.IsMethodCallList(call)) In(call, true);
                else NotLike(call);

                return node;
            }

            Visit(node.Left);

            _sqlCmd.Append(node.GetExpressionType());

            Visit(node.Right);

            return node;
        }

        /// <summary>
        /// Visit Constant for Where Expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            SetParam(node.Value);

            return node;
        }

        /// <summary>
        /// Visit Method Call for Where Expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Contains" && typeof(IEnumerable).IsAssignableFrom(node.Method.DeclaringType) &&
                node.Method.DeclaringType != typeof(string))
                In(node);
            else if (node.Method.Name == "ToLower") ToLower(node);
            else if (node.Method.Name == "ToUpper") ToUpper(node);
            else if (node.Method.Name == "Equals") 
                Equal(node);
            else
                Like(node);

            return node;
        }

        /// <summary>
        /// Set Param for Where Expression
        /// </summary>
        /// <param name="value"></param>
        private void SetParam(object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(TempFieldName))
                {
                    _sqlCmd.Append(ParamName);
                    Param.Add(TempFieldName, value);
                }
            }
            else
            {
                _sqlCmd.Append("NULL");
            }
        }

        /// <summary>
        /// Not Like in Where Expression
        /// </summary>
        /// <param name="node"></param>
        private void NotLike(MethodCallExpression node)
        {
            Visit(node.Object);
            _sqlCmd.AppendFormat(" NOT LIKE {0}", ParamName);
            LikeHelper(node);
        }

        /// <summary>
        /// Like in Where Expression
        /// </summary>
        /// <param name="node"></param>
        /// <exception cref="DapperExtensionException"></exception>
        private void Like(MethodCallExpression node)
        {
            Visit(node.Object);
            _sqlCmd.AppendFormat(" LIKE {0}", ParamName);
            LikeHelper(node);
        }

        private void LikeHelper(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "StartsWith":
                    {
                        ConstantExpression argumentExpression = (ConstantExpression)node.Arguments[0];
                        Param.Add(TempFieldName, argumentExpression.Value + "%");
                    }
                    break;
                case "EndsWith":
                    {
                        ConstantExpression argumentExpression = (ConstantExpression)node.Arguments[0];
                        Param.Add(TempFieldName, "%" + argumentExpression.Value);
                    }
                    break;
                case "Contains":
                    {
                        ConstantExpression argumentExpression = (ConstantExpression)node.Arguments[0];
                        Param.Add(TempFieldName, "%" + argumentExpression.Value + "%");
                    }
                    break;
                default:
                    throw new DapperExtensionException("The expression is not supported by this function");
            }
        }

        /// <summary>
        /// Equal function in Where Expression
        /// </summary>
        /// <param name="node"></param>
        private void Equal(MethodCallExpression node)
        {
            Visit(node.Object);
            _sqlCmd.AppendFormat(" = {0}", ParamName);
            object argumentExpression = node.Arguments[0].ToConvertAndGetValue();
            Param.Add(TempFieldName, argumentExpression);
        }

        /// <summary>
        /// TOLOWER sql function
        /// </summary>
        /// <param name="node"></param>
        private void ToLower(MethodCallExpression node)
        {
            _sqlCmd.Append("LOWER(");
            Visit(node.Object);
            _sqlCmd.Append(")");
        }

        /// <summary>
        /// UPPER sql function
        /// </summary>
        /// <param name="node"></param>
        private void ToUpper(MethodCallExpression node)
        {
            _sqlCmd.Append("UPPER(");
            Visit(node.Object);
            _sqlCmd.Append(")");
        }

        /// <summary>
        /// IN sql function
        /// </summary>
        /// <param name="node"></param>
        private void In(MethodCallExpression node, bool notIn = false)
        {
            IList arrayValue = (IList)((ConstantExpression)node.Object).Value;
            if (arrayValue.Count == 0)  
            {
                _sqlCmd.Append(" 1 = 2");
                return;
            }

            Visit(node.Arguments[0]);

            List<string> paramNames = new List<string>();
            for (int i = 0; i < arrayValue.Count; i++)
            {
                paramNames.Add(ParamName);
                Param.Add(TempFieldName, arrayValue[i]);
                if (i < arrayValue.Count - 1) ParameterCount++;
            }
            string paramName = $"({string.Join(", ", paramNames)})";

            List<string> keywords  = new List<string>();
            if (notIn) keywords.Add("NOT");
            keywords.Add("IN");
            keywords.Add(paramName);
            _sqlCmd.AppendFormat(" {0}", string.Join(" ", keywords));
        }
    }
}
