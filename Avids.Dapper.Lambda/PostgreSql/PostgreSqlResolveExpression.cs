using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.PostgreSql
{
    internal class PostgreSqlResolveExpression : ResolveExpression
    {
        public PostgreSqlResolveExpression(ProviderOption providerOption) : base(providerOption) { }

        /// <summary>
        /// 
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
