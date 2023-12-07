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
                .InnerJoin(typeof(Cashier),
                (SearchInvoiceList inv, Cashier cashier) => inv.CashierId == cashier.Id)
                .InnerJoin(typeof(Customer),
                (SearchInvoiceList inv, Customer cust) => inv.CustomerId == cust.Id)
                .InnerJoin(typeof(InvoiceStatus),
                (Invoice inv, InvoiceStatus stat) => inv.StatusId == stat.Id)
                .InnerJoin(typeof(PaymentStatus),
                (Invoice inv, PaymentStatus stat) => inv.PaymentStatusId == stat.Id)
                .Select(inv => inv)
                .Select(inv => new Cashier
                {
                    Id = inv.CashierId,
                    Name = inv.CreatedBy
                })
                .Select(inv => new Customer
                {
                    Id = inv.CustomerId,
                    Name = inv.CustomerName
                })
                .Select(inv => new InvoiceStatus
                {
                    Id = inv.StatusId,
                    Name = inv.StatusName
                })
                .Select(inv => new PaymentStatus
                {
                    Id = inv.PaymentStatusId,
                    Name = inv.PaymentStatusName
                }).Distinct();

            IEnumerable<SearchInvoiceList> result = await invoices.ToListAsync();

            return result;
        }
    }
}
