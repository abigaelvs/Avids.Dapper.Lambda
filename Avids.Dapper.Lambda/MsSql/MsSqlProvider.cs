namespace Avids.Dapper.Lambda.MsSql
{
    /// <summary>
    /// Sql Provider for Microsoft SQL Server
    /// </summary>
    internal class MsSqlProvider : SqlProvider
    {
        private const char OpenQuote = '[';
        private const char CloseQuote = ']';
        private const char ParameterPrefix = '@';
        private const string FunctionIsNull = "ISNULL";
        private const string FunctionNoLock = "(NOLOCK)";

        public MsSqlProvider()
        {
            ProviderOption = new(OpenQuote, CloseQuote, ParameterPrefix, FunctionIsNull, FunctionNoLock);
            ResolveExpression = new MsSqlResolveExpression(ProviderOption);
        }
    }
}
