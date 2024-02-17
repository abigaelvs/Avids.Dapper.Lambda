using Npgsql;

using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.Test.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly string cs;
        public InvoiceService(IConfiguration conf)
        {
            cs = conf.GetConnectionString("Default");
        }

        public async Task<Invoice> Delete(long id)
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            Invoice result = await conn.QuerySet<Invoice>().Where(c => c.Id.Equals(id)).GetAsync();
            await conn.CommandSet<Invoice>().Where(c => c.Id.Equals(id)).DeleteAsync();

            await conn.CloseAsync();

            return result;
        }

        public async Task<IEnumerable<Invoice>> GetAll()
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            List<Invoice> result = await conn.QuerySet<Invoice>().ToListAsync();
            await conn.CloseAsync();

            return result;
        }

        public async Task<Invoice> GetById(long id)
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            Invoice result = await conn.QuerySet<Invoice>().Where(c => c.Id.Equals(id)).GetAsync();
            await conn.CloseAsync();

            return result;
        }

        public async Task<Invoice> Insert(Invoice customer)
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            await conn.CommandSet<Invoice>().InsertAsync(customer);

            await conn.CloseAsync();

            return customer;
        }

        public async Task<Invoice> Update(Invoice customer)
        {
            using NpgsqlConnection conn = new(cs);

            await conn.OpenAsync();

            await conn.CommandSet<Invoice>().Where(c => c.Id.Equals(customer.Id))
                .UpdateAsync(customer);

            await conn.CloseAsync();

            return customer;
        }
    }
}
