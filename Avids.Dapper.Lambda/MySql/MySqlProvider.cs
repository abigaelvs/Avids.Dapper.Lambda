namespace Avids.Dapper.Lambda.MySql
{
    internal class MySqlProvider : SqlProvider
    {
        private const char OpenQuote = '`';
        private const char CloseQuote = '`';
        private const char ParameterPrefix = '@';
        private const string FunctionIsNull = "IFNULL";
        private const string FunctionNoLock = "NOWAIT";

        public MySqlProvider()
        {
            ProviderOption = new(OpenQuote, CloseQuote, ParameterPrefix, FunctionIsNull, FunctionNoLock);
            ResolveExpression = new MySqlResolveExpression(ProviderOption);
        }
    }
}
