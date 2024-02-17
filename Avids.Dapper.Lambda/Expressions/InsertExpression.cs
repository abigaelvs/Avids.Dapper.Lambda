using System.Linq;
using System.Collections;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Avids.Dapper.Lambda.Extension;
using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Expressions
{
    /// <summary>
    /// Insert Expression Builder
    /// </summary>
    public class InsertExpression : SqlCmdExpression
    {
        public override string SqlCmd => _sqlCmd.Length > 0 ? _sqlCmd.ToString() : string.Empty;


        public string FieldsStr
        {
            get => string.Join(",", Fields);
        }
        private List<string> Fields { get; set; } = new List<string>();

        public string ValuesStr
        {
            get => string.Join(",", Values);
        }
        private List<string> Values { get; set; } = new List<string>();
        private List<string> TempValues { get; set; } = new List<string>();
        private int ParameterCount { get; set; } = 1;
        public InsertExpression(LambdaExpression expression, ProviderOption providerOption)
            : base(null, providerOption)
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

            object exprValue = ((ConstantExpression)TrimExpression.Trim(memberInitExpression)).Value;

            bool isBulkInsert = typeof(IList).IsAssignableFrom(exprValue.GetType());

            List<object> entities = new List<object>() { };

            if (isBulkInsert)
            {
                IList listValue = (IList)exprValue;
                foreach (object item in listValue) entities.Add(item);
            }
            else
            {
                entities.Add(exprValue);
            }

            bool needParameterCount = entities.Count > 1;
            foreach (object e in entities)
            {
                bool needField = Fields.Count < 1;
                PropertyInfo[] properties = e.GetType().GetProperties();
                foreach (PropertyInfo item in properties)
                {
                    // Check if property has DatabaseGenerated attribute, if yes skip (not insert)
                    if (item.CustomAttributes.Any(b => b.AttributeType == typeof(DatabaseGeneratedAttribute)))
                        continue;

                    object value = item.GetValue(e);
                    string paramName = item.Name;
                    string count = needParameterCount ? ParameterCount.ToString() : "";
                    string valueParam = $"{_parameterPrefix}{_prefix}{paramName}{count}";
                    TempValues.Add(valueParam);
                    Param.Add(valueParam, value);

                    if (needField)
                    {
                        string fieldName = _providerOption.CombineFieldName(item.GetColumnAttributeName());
                        Fields.Add(fieldName);
                    }
                }

                string values = $"({string.Join(",", TempValues)})";
                TempValues.Clear();
                Values.Add(values);
            }

            GenerateSql();

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

                string fieldName = memberAssignment.Member.GetColumnAttributeName();
                string paramName = memberAssignment.Member.Name;
                SetParam(fieldName, paramName, value);
            }

            GenerateSql();

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
            string newFieldName = _providerOption.CombineFieldName(fieldName);
            string newParamName = $"{_parameterPrefix}{_prefix}{paramName}";
            Fields.Add(newFieldName);
            Values.Add(newParamName);
            Param.Add(newParamName, value);
        }

        /// <summary>
        /// Generate SqlCmd
        /// </summary>
        private void GenerateSql()
        {
            _sqlCmd.AppendFormat("({0}) VALUES {1}", FieldsStr, ValuesStr);
        }
    }
}
