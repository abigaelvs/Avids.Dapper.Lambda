using System.Linq.Expressions;
using System.Text;

using Dapper;

using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Expressions
{
    public class SqlCmdExpression : ExpressionVisitor
    {
        public virtual string SqlCmd => _sqlCmd.Length > 0 ? _sqlCmd.ToString() : string.Empty;
        protected readonly StringBuilder _sqlCmd;
        public DynamicParameters Param { get; }
        protected readonly string _prefix;
        protected readonly char _parameterPrefix;
        protected readonly ProviderOption _providerOption;

        public SqlCmdExpression(string prefix, ProviderOption providerOption)
        {
            _sqlCmd = new StringBuilder(100);
            Param = new DynamicParameters();

            _prefix = prefix;
            _providerOption = providerOption;
            _parameterPrefix = _providerOption.ParameterPrefix;
        }
    }
}
