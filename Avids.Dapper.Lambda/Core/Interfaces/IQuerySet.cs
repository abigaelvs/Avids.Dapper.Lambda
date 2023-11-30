using System.Linq.Expressions;
using Avids.Dapper.Lambda.Core.SetQ;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IQuerySet<T>
    {
        QuerySet<T> Where(Expression<Func<T, bool>> predicate);

        QuerySet<T> WithNoLock();
    }
}
