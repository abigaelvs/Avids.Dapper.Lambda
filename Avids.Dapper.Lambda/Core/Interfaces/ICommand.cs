using System.Linq.Expressions;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface ICommand<T>
    {
        int Update(T entity);

        Task<int> UpdateAsync(T entity);

        int Update(Expression<Func<T, T>> updateExpression);

        Task<int> UpdateAsync(Expression<Func<T, T>> updateExpression);

        int Delete();

        Task<int> DeleteAsync();
    }
}
