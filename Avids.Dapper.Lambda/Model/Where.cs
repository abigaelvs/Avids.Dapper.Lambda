using System.Linq.Expressions;

namespace Avids.Dapper.Lambda.Model
{
    public class Where
    {
        public EWhere? WhereType { get; set; }
        public LambdaExpression WhereExpression { get; set; }
    }
}
