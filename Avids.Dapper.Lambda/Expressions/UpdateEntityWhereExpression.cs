using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

using Avids.Dapper.Lambda.Extension;
using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Expressions
{
    public class UpdateEntityWhereExpression : SqlCmdExpression
    {
        public override string SqlCmd => _sqlCmd.Length > 0 ? $" WHERE {_sqlCmd} " : string.Empty;

        private List<string> Fields { get; set; } = new List<string>();

        /// <summary>
        /// Update Entity Where Expression Builder
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public UpdateEntityWhereExpression(LambdaExpression expression, ProviderOption providerOption)
            : base("", providerOption)
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
                // Check if property has DatabaseGenerated attribute, if yes skip (not insert)
                Console.WriteLine(item.Name);

                if (!item.CustomAttributes.Any(b => b.AttributeType == typeof(KeyAttribute)) && 
                    !item.CustomAttributes.Any(b => b.AttributeType == typeof(DatabaseGeneratedAttribute)))
                    continue;

                object value = item.GetValue(entity);
                string fieldName = _providerOption.CombineFieldName(item.GetColumnAttributeName());
                string paramName = $"{_parameterPrefix}{_prefix}{item.Name}";
                Fields.Add($"{fieldName} = {paramName}");
                Param.Add(paramName, value);
            }

            _sqlCmd.Append(string.Join(" AND ", Fields));

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

                if (!memberAssignment.Member.CustomAttributes.Any(b => b.AttributeType == typeof(KeyAttribute)) && 
                    !memberAssignment.Member.CustomAttributes.Any(b => b.AttributeType == typeof(DatabaseGeneratedAttribute)))
                    continue;

                object value = null;
                switch (memberAssignment.Expression.NodeType)
                {
                    case ExpressionType.Constant:
                        ConstantExpression constantExpression = (ConstantExpression)memberAssignment.Expression;
                        value = constantExpression.Value;
                        break;
                    case ExpressionType.MemberAccess:
                        object constantValue = ((MemberExpression)memberAssignment.Expression).MemberToValue();
                        value = constantValue;
                        break;
                }

                string fieldName = _providerOption.CombineFieldName(memberAssignment.Member.GetColumnAttributeName());
                string paramName = $"{_parameterPrefix}{_prefix}{memberAssignment.Member.Name}";
                Fields.Add($"{fieldName} = {paramName}");
                Param.Add(paramName, value);
            }

            _sqlCmd.Append(string.Join(" AND ", Fields));
            return node;
        }
    }
}