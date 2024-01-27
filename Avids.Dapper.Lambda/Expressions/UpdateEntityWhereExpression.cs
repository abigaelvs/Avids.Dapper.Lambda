using System.Reflection;

using Avids.Dapper.Lambda.Extension;
using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Expressions
{
    public class UpdateEntityWhereExpression : SqlCmdExpression
    {
        public override string SqlCmd => _sqlCmd.Length > 0 ? $" WHERE {_sqlCmd} " : string.Empty;

        private readonly object _obj;

        /// <summary>
        /// Update Entity Where Expression Builder
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public UpdateEntityWhereExpression(object obj, ProviderOption providerOption)
            : base("", providerOption)
        {
            _obj = obj;
            Resolve();
        }

        /// <summary>
        /// Resolve Update Entity Where Expression
        /// </summary>
        public void Resolve()
        {
            PropertyInfo[] propertyInfos = _obj.GetKeyProperties();
            int i = 0;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (i > 0) _sqlCmd.Append(" AND ");
                string fieldName = _providerOption.CombineFieldName(propertyInfo.GetColumnAttributeName());
                _sqlCmd.Append(fieldName);
                _sqlCmd.Append(" = ");
                SetParam(propertyInfo.Name, propertyInfo.GetValue(_obj));
                i++;
            }
        }

        private void SetParam(string fileName, object value)
        {
            if (value != null)
            {
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    _sqlCmd.Append(_parameterPrefix + fileName);
                    Param.Add(fileName, value);
                }
            }
            else
            {
                _sqlCmd.Append("NULL");
            }
        }
    }
}