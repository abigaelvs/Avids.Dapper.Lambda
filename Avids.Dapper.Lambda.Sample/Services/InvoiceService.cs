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

            //conn.CommandSet<Invoice>().Update(inv => new Invoice
            //{
            //    Id = 1,
            //    No = "asda",
            //    UpdatedDate = DateTime.Now,
            //});

            PaymentStatus stat = new();
            stat.Id = 1;
            stat.Name = "kwklnad";

            PaymentStatus stat2 = new();
            stat.Id = 2;
            stat.Name = "wkwkland";

            List<PaymentStatus> stats = new();
            stats.Add(stat);
            stats.Add(stat2);

            Invoice inv = new();
            inv.Id = 1;
            inv.No = "IV123";
            inv.CustomerId = 1;
            inv.CashierId = 1;
            inv.StatusId = 1;
            inv.CreatedDate = DateTime.Now;
            inv.UpdatedByUserId = null;
            inv.UpdatedDate = null;

            Invoice inv2 = new();
            inv.Id = 2;
            inv.No = "IV124";
            inv.CustomerId = 1;
            inv.CashierId = 1;
            inv.StatusId = 1;
            inv.CreatedDate = DateTime.Now;
            inv.UpdatedByUserId = null;
            inv.UpdatedDate = null;

            List<Invoice> invoices2 = new();
            invoices2.Add(inv);
            invoices2.Add(inv2);

            List<Invoice> newInvoices = invoices2.Select(inv => new Invoice
            {
                Id = inv.Id,
                No = inv.No,
            }).ToList();

            conn.CommandSet<Invoice>().Insert(invoices2);

            //conn.CommandSet<PaymentStatus>().Insert(stats);

            Option<SearchInvoiceList> invoices = conn.QuerySet<SearchInvoiceList>()
                .InnerJoin((InvoiceStatus stat, SearchInvoiceList inv) => stat.Id == inv.Id)
                .LeftJoin((Cashier cashier, SearchInvoiceList inv) => cashier.Id == inv.CashierId)
                .RightJoin((Customer cust, SearchInvoiceList inv) => cust.Id == inv.CustomerId)
                .FullJoin((PaymentStatus stat, SearchInvoiceList inv) => stat.Id == inv.PaymentStatusId)
                .GroupBy(inv => inv.Id).OrderBy(inv => inv.Id)
                .Select(inv => inv.Id)
                //.Select(inv => new Cashier { Id = inv.CashierId, Name = inv.CreatedBy })
                //.Select(inv => new Customer { Id = inv.CustomerId, Name = inv.CustomerName })
                //.Select(inv => new InvoiceStatus { Id = inv.StatusId, Name = inv.StatusName })
                //.Select(inv => new PaymentStatus{ Id = inv.PaymentStatusId, Name = inv.PaymentStatusName })
                .Distinct().Limit(1).Offset(10);

            IEnumerable<SearchInvoiceList> result = await invoices.ToListAsync();

            return result;
        }
    }
}
