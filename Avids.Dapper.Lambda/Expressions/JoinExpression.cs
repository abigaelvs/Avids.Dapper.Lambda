using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Expressions
{
    public class JoinExpression : WhereExpression
    {
        /// <inheritdoc/>
        public override string SqlCmd => _sqlCmd.Length > 0 ? $"{_sqlCmd}" : string.Empty;
        public JoinExpression(Queue<Join> joinExpressions, string prefix, ProviderOption providerOption,
            bool withTableName = false) : base(prefix, providerOption, withTableName) 
        {
            while (joinExpressions.Count > 0)
            {
                Join curr = joinExpressions.Dequeue();
                string tableName = providerOption.CombineFieldName(curr.TableType.GetTableAttributeName());

                _sqlCmd.Append($" {curr.JoinType} {tableName} ON ");
                Visit(TrimExpression.Trim(curr.OnExpression));
            }
        }
    }
}
