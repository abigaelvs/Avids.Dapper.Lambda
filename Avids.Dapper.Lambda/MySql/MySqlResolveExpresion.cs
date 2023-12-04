using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.MySql
{
    /// <summary>
    /// Expression Resolver for MySql
    /// </summary>
    internal class MySqlResolveExpression : ResolveExpression
    {
        public MySqlResolveExpression(ProviderOption providerOption) : base(providerOption) { }

        public override string ResolveWithNoLock(bool nolock)
        {
            return "";
        }
    }
}
