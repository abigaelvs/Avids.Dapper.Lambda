using Npgsql;

using Avids.Dapper.Lambda.Core.SetQ;
using Avids.Dapper.Lambda.Sample.Entity;
using Avids.Dapper.Lambda.Sample.Models;

namespace Avids.Dapper.Lambda.Sample.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly string cs;
        public InvoiceService(IConfiguration conf)
        {
            cs = conf.GetConnectionString("Default");
        }

        public async Task<IEnumerable<SearchInvoiceList>> SearchInvoice(SearchInvoiceRequestDto request)
        {
            using NpgsqlConnection conn = new(cs);

            Option<SearchInvoiceList> invoices = conn.QuerySet<SearchInvoiceList>()
                .InnerJoin((InvoiceStatus stat, SearchInvoiceList inv) => stat.Id == inv.Id)
                .LeftJoin((Cashier cashier, SearchInvoiceList inv) => cashier.Id == inv.CashierId)
                .RightJoin((Customer cust, SearchInvoiceList inv) => cust.Id == inv.CustomerId)
                .FullJoin((PaymentStatus stat, SearchInvoiceList inv) => stat.Id == inv.PaymentStatusId)
                .GroupBy(inv => inv.No).OrderBy(inv => inv.Id)
                .Select(inv => inv)
                .Select(inv => new Cashier { Id = inv.CashierId, Name = inv.CreatedBy })
                .Select(inv => new Customer { Id = inv.CustomerId, Name = inv.CustomerName })
                .Select(inv => new InvoiceStatus { Id = inv.StatusId, Name = inv.StatusName })
                .Select(inv => new PaymentStatus{ Id = inv.PaymentStatusId, Name = inv.PaymentStatusName })
                .Distinct().Limit(1).Offset(10);

            IEnumerable<SearchInvoiceList> result = await invoices.ToListAsync();

            return result;
        }
    }
}
