using Avids.Dapper.Lambda.Sample.Entity;

namespace Avids.Dapper.Lambda.Sample.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAll();
        Task<Customer> GetById(long id);
        Task<Customer> Insert(Customer customer);
        Task<Customer> Update(Customer customer);
        Task<Customer> Delete(long id);
    }
}
