using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Model;


namespace Avids.Dapper.Lambda.Helper
{
    public class WhereBuilder<T>
    {
        public Queue<Where> WhereExpressions { get; } = new Queue<Where>();

        public WhereBuilder<T> And(Expression<Func<T, bool>> predicate)
        {
            Where where = new Where();
            if (WhereExpressions.Count > 0) where.WhereType = EWhere.AND;
            where.WhereExpression = predicate;
            WhereExpressions.Enqueue(where);
            return this;
        }

        public WhereBuilder<T> And<W>(Expression<Func<W, bool>> predicate)
        {
            Where where = new Where();
            if (WhereExpressions.Count > 0) where.WhereType = EWhere.AND;
            where.WhereExpression = predicate;
            WhereExpressions.Enqueue(where);
            return this;
        }
    }
}
