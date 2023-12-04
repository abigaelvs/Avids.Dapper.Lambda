using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.SqLite
{
    internal class SqLiteResolveExpression : ResolveExpression
    {
        public SqLiteResolveExpression(ProviderOption providerOption) : base(providerOption) { }

        /// <summary>
        /// No Lock for SqlLite (Not supported)
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
