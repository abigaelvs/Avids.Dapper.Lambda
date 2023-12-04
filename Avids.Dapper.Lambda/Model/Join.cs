using System;
using System.Linq.Expressions;

namespace Avids.Dapper.Lambda.Model
{
    /// <summary>
    /// Class for Join
    /// </summary>
    public class Join
    {
        public Type TableType { get; set; }
        public string JoinType { get; set; }
        public LambdaExpression OnExpression { get; set; }
    }
}
