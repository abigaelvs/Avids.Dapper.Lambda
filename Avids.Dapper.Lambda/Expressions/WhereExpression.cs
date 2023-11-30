using System.Collections;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Extension;
using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Exception;
using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Expressions
{
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

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);

            _sqlCmd.Append(node.GetExpressionType());

            Visit(node.Right);

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            SetParam(node.Value);

            return node;
        }

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

        private void Like(MethodCallExpression node)
        {
            Visit(node.Object);
            _sqlCmd.AppendFormat(" LIKE {0}", ParamName);
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

        private void Equal(MethodCallExpression node)
        {
            Visit(node.Object);
            _sqlCmd.AppendFormat(" = {0}", ParamName);
            object argumentExpression = node.Arguments[0].ToConvertAndGetValue();
            Param.Add(TempFieldName, argumentExpression);
        }

        private void ToLower(MethodCallExpression node)
        {
            _sqlCmd.Append("LOWER(");
            Visit(node.Object);
            _sqlCmd.Append(")");
        }

        private void ToUpper(MethodCallExpression node)
        {
            _sqlCmd.Append("UPPER(");
            Visit(node.Object);
            _sqlCmd.Append(")");
        }

        private void In(MethodCallExpression node)
        {
            IList arrayValue = (IList)((ConstantExpression)node.Object).Value;
            if (arrayValue.Count == 0)
            {
                _sqlCmd.Append(" 1 = 2");
                return;
            }
            Visit(node.Arguments[0]);
            _sqlCmd.AppendFormat(" IN {0}", ParamName);
            Param.Add(TempFieldName, arrayValue);
        }
    }
}
