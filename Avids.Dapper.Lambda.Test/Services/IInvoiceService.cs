using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.Test.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAll();
        Task<Invoice> GetById(long id);
        Task<Invoice> Insert(Invoice customer);
        Task<Invoice> Update(Invoice customer);
        Task<Invoice> Delete(long id);
    }
}
