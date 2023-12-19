using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Avids.Dapper.Lambda.Model
{
    internal class SetContext
    {
        public SetContext()
        {
            OrderbyExpressionList = new Dictionary<EOrderBy, LambdaExpression>();
        }

        /// <summary>
        /// Entity / Table class
        /// </summary>
        public Type TableType { get; internal set; }

        /// <summary>
        /// Queue of Where
        /// </summary>
        public Queue<Where> WhereExpressions { get; internal set; } = new Queue<Where>();

        /// <summary>
        /// If Not Exists Expression
        /// </summary>
        public LambdaExpression IfNotExistsExpression { get; internal set; }

        /// <summary>
        /// Order By Expression List
        /// </summary>
        public Dictionary<EOrderBy, LambdaExpression> OrderbyExpressionList { get; internal set; }

        /// <summary>
        /// Group By Expression List
        /// </summary>
        public Queue<LambdaExpression> GroupByExpressionList { get; internal set; } = new Queue<LambdaExpression>();

        /// <summary>
        /// Queue of Select Expression
        /// </summary>
        public Queue<Select> SelectExpressions { get; internal set; } = new Queue<Select>();

        /// <summary>
        /// Queue of Join Expression
        /// </summary>
        public Queue<Join> JoinExpressions { get; internal set; } = new Queue<Join>();

        /// <summary>
        /// Limit Num
        /// </summary>
        public int? LimitNum { get; internal set; }

        /// <summary>
        /// Offset Num
        /// </summary>
        public int? OffsetNum { get; internal set; }

        /// <summary>
        /// No Lock
        /// </summary>
        public bool NoLock { get; internal set; }

        /// <summary>
        /// Distinct
        /// </summary>
        public bool Distinct { get; internal set; }

        /// <summary>
        /// Get if context has join expression or not
        /// </summary>
        public bool HasJoin { get; set; }
    }
}
