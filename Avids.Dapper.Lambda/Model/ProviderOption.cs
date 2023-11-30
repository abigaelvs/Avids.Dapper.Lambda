namespace Avids.Dapper.Lambda.Model
{
    public class ProviderOption
    {
        public ProviderOption(char openQuote, char closeQuote, char parameterPrefix, string functionIsNull,
            string functionNoLock)
        {
            OpenQuote = openQuote;
            CloseQuote = closeQuote;
            ParameterPrefix = parameterPrefix;
            FunctionIsNull = functionIsNull;
            FunctionNoLock = functionNoLock;
        }

        /// <summary>
        /// Open Quote of Table and Column
        /// </summary>
        public char OpenQuote { get; set; }

        /// <summary>
        /// Close Quote of Table and Column
        /// </summary>
        public char CloseQuote { get; set; }

        /// <summary>
        /// Parameter Prefix
        /// </summary>
        public char ParameterPrefix { get; set; }

        /// <summary>
        /// IS NULL sql function
        /// </summary>
        public string FunctionIsNull { get; set; }

        /// <summary>
        /// NO LOCK sql function
        /// </summary>
        public string FunctionNoLock { get; set; }

        /// <summary>
        /// Combine field name with Open Quote and Close Quote
        /// </summary>
        /// <param name="field"></param>
        /// <param name="noQuote"></param>
        /// <returns></returns>
        public string CombineFieldName(string field, bool noQuote = false)
        {
            if (noQuote) return field;
            else return OpenQuote + field + CloseQuote;
        }
    }
}
