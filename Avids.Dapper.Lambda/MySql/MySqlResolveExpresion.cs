using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.MySql
{
    internal class MySqlResolveExpression : ResolveExpression
    {
        public MySqlResolveExpression(ProviderOption providerOption) : base(providerOption) { }

        public override string ResolveWithNoLock(bool nolock)
        {
            return "";
        }
    }
}
