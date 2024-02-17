using Avids.Dapper.Lambda.Test.Entity;
using Npgsql;

namespace Avids.Dapper.Lambda.Test.Tests
{
    public class JoinTest
    {
        [Fact]
        public void TestJoin()
        {
            string expected = @"SELECT  ""Cashier"".""Id"" AS ""CashierId"", ""Cashier"".""Name"" AS ""CreatedBy"", "
                + @"""Customer"".""Id"" AS ""CustomerId"", ""Customer"".""Name"" AS ""CustomerName"", "
                + @"""InvoiceStatus"".""Id"" AS ""StatusId"", ""InvoiceStatus"".""Name"" AS ""StatusName"", "
                + @"""PaymentStatus"".""Id"" AS ""PaymentStatusId"", ""PaymentStatus"".""Name"" AS ""PaymentStatusName"" "
                + @"FROM ""Invoice""  "
                + @"INNER JOIN ""InvoiceStatus"" ON ""InvoiceStatus"".""Id"" = ""Invoice"".""Id"" "
                + @"LEFT JOIN ""Cashier"" ON ""Cashier"".""Id"" = ""Invoice"".""CashierId"" "
                + @"RIGHT JOIN ""Customer"" ON ""Customer"".""Id"" = ""Invoice"".""CustomerId"" "
                + @"FULL JOIN ""PaymentStatus"" ON ""PaymentStatus"".""Id"" = ""Invoice"".""PaymentStatusId""  "
                + @"WHERE ""InvoiceStatus"".""Id"" = @Id1";
            string actual = new NpgsqlConnection().QuerySet<SearchInvoiceList>()
                .InnerJoin((InvoiceStatus stat, SearchInvoiceList inv) => stat.Id == inv.Id)
                .LeftJoin((Cashier cashier, SearchInvoiceList inv) => cashier.Id == inv.CashierId)
                .RightJoin((Customer cust, SearchInvoiceList inv) => cust.Id == inv.CustomerId)
                .FullJoin((PaymentStatus stat, SearchInvoiceList inv) => stat.Id == inv.PaymentStatusId)
                .Where((InvoiceStatus stat) => stat.Id == 1)
                .Select(inv => new Cashier { Id = inv.CashierId, Name = inv.CreatedBy })
                .Select(inv => new Customer { Id = inv.CustomerId, Name = inv.CustomerName })
                .Select(inv => new InvoiceStatus { Id = inv.StatusId, Name = inv.StatusName })
                .Select(inv => new PaymentStatus { Id = inv.PaymentStatusId, Name = inv.PaymentStatusName })
                .SqlProvider.FormatToList<SearchInvoiceList>().SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
