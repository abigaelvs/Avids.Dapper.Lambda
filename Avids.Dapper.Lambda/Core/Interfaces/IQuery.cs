using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IQuery<T>
    {
        T Get();

        Task<T> GetAsync();

        IEnumerable<T> ToList();

        Task<IEnumerable<T>> ToListAsync();

        PageList<T> PageList(int pageIndex, int pageSize);
    }
}
