using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.MsSql
{
    /// <summary>
    /// Expression Resolver for Microsoft SQL Server
    /// </summary>
    internal class MsSqlResolveExpression : ResolveExpression
    {
        public MsSqlResolveExpression(ProviderOption providerOption) : base(providerOption) { }
    }
}
