using Avids.Dapper.Lambda;

namespace Avids.Dapper.Lambda.SqLite
{
    /// <summary>
    /// Sql Format Provider for SQLite Database
    /// </summary>
    internal class SqLiteProvider : SqlProvider
    {
        /// <summary>
        /// Open Quote / Start Quote for table name or column name
        /// </summary>
        private const char OpenQuote = '"';

        /// <summary>
        /// Close Quote / End Quote for table name or column name
        /// </summary>
        private const char CloseQuote = '"';

        /// <summary>
        /// Prefix for statement parameter
        /// </summary>
        private const char ParameterPrefix = '$';

        /// <summary>
        /// Sql Function for check if column is null
        /// </summary>
        private const string FunctionIsNull = "COALESCE";

        /// <summary>
        /// Sql Function for check is NOLOCK
        /// </summary>
        private const string FunctionNoLock = "NOLOCK";

        public SqLiteProvider() : base()
        {
            ProviderOption = new(OpenQuote, CloseQuote, ParameterPrefix, FunctionIsNull, FunctionNoLock);
            ResolveExpression = new SqLiteResolveExpression(ProviderOption);
        }
    }
}
