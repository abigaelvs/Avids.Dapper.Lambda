using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

using Avids.Dapper.Lambda.Extension;
using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Expressions
{
    /// <summary>
    /// Update Expression Builder
    /// </summary>
    public class UpdateExpression : SqlCmdExpression
    {
        public override string SqlCmd => _sqlCmd.Length > 0 ? $" SET {_sqlCmd} " : string.Empty;

        public UpdateExpression(LambdaExpression expression, ProviderOption providerOption)
            : base("UPDATE_", providerOption)
        {
            Visit(expression);
        }

        /// <summary>
        /// Visit Member for Update Expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            MemberExpression memberInitExpression = node;

            object entity = ((ConstantExpression)TrimExpression.Trim(memberInitExpression)).Value;

            PropertyInfo[] properties = memberInitExpression.Type.GetProperties();

            foreach (PropertyInfo item in properties)
            {
                if (item.CustomAttributes.Any(b => b.AttributeType == typeof(KeyAttribute)))
                    continue;

                if (_sqlCmd.Length > 0)
                    _sqlCmd.Append(",");

                string paramName = item.Name;
                object value = item.GetValue(entity);
                string fieldName = _providerOption.CombineFieldName(item.GetColumnAttributeName());
                SetParam(fieldName, paramName, value);
            }

            return node;
        }

        /// <summary>
        /// Visit Member Init for Update Expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            MemberInitExpression memberInitExpression = node;

            foreach (MemberBinding item in memberInitExpression.Bindings)
            {
                MemberAssignment memberAssignment = (MemberAssignment)item;

                if (_sqlCmd.Length > 0)
                    _sqlCmd.Append(",");

                string paramName = memberAssignment.Member.Name;
                string fieldName = _providerOption.CombineFieldName(memberAssignment.Member.GetColumnAttributeName());
                switch (memberAssignment.Expression.NodeType)
                {
                    case ExpressionType.Constant:
                        ConstantExpression constantExpression = (ConstantExpression)memberAssignment.Expression;
                        SetParam(fieldName, paramName, constantExpression.Value);
                        break;
                    case ExpressionType.MemberAccess:
                        object constantValue = ((MemberExpression)memberAssignment.Expression).MemberToValue();
                        SetParam(fieldName, paramName, constantValue);
                        break;
                    case ExpressionType.Convert:
                        SetParam(fieldName, paramName, memberAssignment.Expression.ToConvertAndGetValue());
                        break;
                }
            }

            return node;
        }

        /// <summary>
        /// Set Param for sql cmd
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        private void SetParam(string fieldName, string paramName, object value)
        {
            string n = $"{_parameterPrefix}{_prefix}{paramName}";
            _sqlCmd.AppendFormat(" {0}={1} ", fieldName, n);
            Param.Add(n, value);
        }
    }
}
