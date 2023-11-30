namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IInsert<T>
    {
        int Insert(T entity);

        Task<int> InsertAsync(T entity);
    }
}
