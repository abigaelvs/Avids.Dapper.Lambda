using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.PostgreSql
{
    /// <summary>
    /// Expression Resolver for PostgreSql
    /// </summary>
    internal class PostgreSqlResolveExpression : ResolveExpression
    {
        public PostgreSqlResolveExpression(ProviderOption providerOption) : base(providerOption) { }

        /// <summary>
        /// No Lock in PostgreSql (Not Supported)
        /// </summary>
        /// <param name="nolock"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override string ResolveWithNoLock(bool nolock)
        {
            return "";
        }
    }
}
