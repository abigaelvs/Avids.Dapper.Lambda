using Avids.Dapper.Lambda.Sample.Entity;

using Npgsql;

namespace Avids.Dapper.Lambda.Sample.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly string cs;
        public CustomerService(IConfiguration conf) 
        {
            cs = conf.GetConnectionString("Default");
        }

        public async Task<Customer> Delete(long id)
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            Customer result = await conn.QuerySet<Customer>().Where(c => c.Id.Equals(id)).GetAsync();
            await conn.CommandSet<Customer>().Where(c => c.Id.Equals(id)).DeleteAsync();

            await conn.CloseAsync();

            return result;
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            List<Customer> result = await conn.QuerySet<Customer>().ToListAsync();
            await conn.CloseAsync();

            return result;
        }

        public async Task<Customer> GetById(long id)
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            Customer result = await conn.QuerySet<Customer>().Where(c => c.Id.Equals(id)).GetAsync();
            await conn.CloseAsync();

            return result;
        }

        public async Task<Customer> Insert(Customer customer)
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            await conn.CommandSet<Customer>().InsertAsync(customer);

            await conn.CloseAsync();

            return customer;
        }

        public async Task<Customer> Update(Customer customer)
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            await conn.CommandSet<Customer>().Where(c => c.Id.Equals(customer.Id))
                .UpdateAsync(customer);

            await conn.CloseAsync();

            return customer;
        }
    }
}
