using Avids.Dapper.Lambda.Test.Entity;

using Npgsql;

namespace Avids.Dapper.Lambda.Test.Tests
{
    public class SqlProviderTest
    {
        // [Fact]
        public void FormatGetTest()
        {
            List<long> ids = new() { 1, 2, 3 };
            List<string> nos = new() { "IV1", "IV2" };

            //string expected = @"SELECT ""Cashier"".""Id"" AS ""CashierId"", ""Cashier"".""Name"" AS ""CreatedBy"", "
            //    + @"""Customer"".""Id"" AS ""CustomerId"", ""Customer"".""Name"" AS ""CustomerName"", "
            //    + @"""InvoiceStatus"".""Id"" AS ""StatusId"", ""InvoiceStatus"".""Name"" AS ""StatusName"", "
            //    + @"""PaymentStatus"".""Id"" AS ""PaymentStatusId"", ""PaymentStatus"".""Name"" AS ""PaymentStatusName"" "
            //    + @"FROM ""Invoice"" "
            //    + @"INNER JOIN ""InvoiceStatus"" ON ""InvoiceStatus"".""Id"" = ""Invoice"".""Id"" "
            //    + @"LEFT JOIN ""Cashier"" ON ""Cashier"".""Id"" = ""Invoice"".""CashierId"" "
            //    + @"RIGHT JOIN ""Customer"" ON ""Customer"".""Id"" = ""Invoice"".""CustomerId"" "
            //    + @"FULL JOIN ""PaymentStatus"" ON ""PaymentStatus"".""Id"" = ""Invoice"".""PaymentStatusId"" "
            //    + @"WHERE (""Id"" = @Id1 AND ""Id"" = @Id2 ""StatusId"" > @StatusId3 "
            //    + @"AND ""StatusId"" < @StatusId4 OR ""StatusId"" <= @StatusId5 AND ""StatusId"" >= @StatusId6 "
            //    + @"AND ""Id"" != @Id7 OR ""No"" LIKE @No7 AND ""No"" LIKE @No8 AND ""No"" LIKE @No9 "
            //    + @"OR ""No"" NOT LIKE @No10 AND ""No"" NOT LIKE @No11 AND ""No"" NOT LIKE @No12 "
            //    + @"OR ""Id"" IN (@Id13, @Id14, @Id15) AND ""No"" IN (@No16, @No17) AND ""No"" IS NULL "
            //    + @"OR ""Id"" NOT IN (@Id19, @Id20, @Id21) AND ""No"" NOT IN (@No22, @No23)) "
            //    + @"GROUP BY ""Invoice"".""No"" "
            //    + @"ORDER BY ""Invoice"".""Id"" ASC, ""No"" DESC LIMIT 1";

            string expected = @"SELECT ""Cashier"".""Id"" AS ""CashierId"", ""Cashier"".""Name"" AS ""CreatedBy"", "
                + @"""Customer"".""Id"" AS ""CustomerId"", ""Customer"".""Name"" AS ""CustomerName"", "
                + @"""InvoiceStatus"".""Id"" AS ""StatusId"", ""InvoiceStatus"".""Name"" AS ""StatusName"", "
                + @"""PaymentStatus"".""Id"" AS ""PaymentStatusId"", ""PaymentStatus"".""Name"" AS ""PaymentStatusName"" "
                + @"FROM ""Invoice"" "
                + @"INNER JOIN ""InvoiceStatus"" ON ""InvoiceStatus"".""Id"" = ""Invoice"".""Id"" "
                + @"LEFT JOIN ""Cashier"" ON ""Cashier"".""Id"" = ""Invoice"".""CashierId"" "
                + @"RIGHT JOIN ""Customer"" ON ""Customer"".""Id"" = ""Invoice"".""CustomerId"" "
                + @"FULL JOIN ""PaymentStatus"" ON ""PaymentStatus"".""Id"" = ""Invoice"".""PaymentStatusId"" "
                + @"WHERE (""Invoice"".""Id"" = @Id1 AND ""Invoice"".""Id"" = @Id2) "
                + @"OR (""Invoice"".""StatusId"" > @StatusId3 AND ""Invoice"".""StatusId"" < @StatusId4 "
                + @"AND ""Invoice"".""StatusId"" <= @StatusId5 AND ""Invoice"".""StatusId"" >= @StatusId6) "
                + @"OR (""Invoice"".""Id"" != @Id7 AND ""Invoice"".""Id"" != @Id8) "
                + @"OR (""Invoice"".""No"" = @No9 AND ""Invoice"".""No"" = @No10) "
                + @"OR (""Invoice"".""No"" != @No11 AND ""Invoice"".""No"" != @No12) "
                + @"OR (""Invoice"".""No"" IS NULL AND ""Invoice"".""No"" IS NULL) "
                + @"OR (""Invoice"".""No"" IS NOT NULL AND ""Invoice"".""No"" IS NOT NULL) "
                + @"OR (""Invoice"".""No"" LIKE @No17 AND ""Invoice"".""No"" LIKE @No18 "
                + @"AND ""Invoice"".""No"" LIKE @No19) "
                + @"OR (""Invoice"".""No"" NOT LIKE @No20 AND ""Invoice"".""No"" NOT LIKE @No21 "
                + @"AND ""Invoice"".""No"" NOT LIKE @No22) "
                + @"OR (LOWER(""Invoice"".""No"") LIKE @No23 AND LOWER(""Invoice"".""No"") LIKE @No24 "
                + @"AND LOWER(""Invoice"".""No"") LIKE @No25) "
                + @"OR (LOWER(""Invoice"".""No"") NOT LIKE @No26 "
                + @"AND LOWER(""Invoice"".""No"") NOT LIKE @No27 "
                + @"AND LOWER(""Invoice"".""No"") NOT LIKE @No28) "
                + @"OR (""Invoice"".""Id"" IN (@Id29, @Id30, @Id31) "
                + @"AND ""Invoice"".""No"" IN (@No32, @No33)) "
                + @"OR (""Invoice"".""Id"" NOT IN (@Id34, @Id35, @Id36) "
                + @"AND ""Invoice"".""No"" NOT IN (@No37, @No38)) "
                + @"GROUP BY ""Invoice"".""No"" "
                + @"ORDER BY ""Invoice"".""Id"" ASC, ""Invoice"".""No"" DESC LIMIT 1";

            string actual = new NpgsqlConnection().QuerySet<SearchInvoiceList>()
                .InnerJoin((InvoiceStatus stat, SearchInvoiceList inv) => stat.Id == inv.Id)
                .LeftJoin((Cashier cashier, SearchInvoiceList inv) => cashier.Id == inv.CashierId)
                .RightJoin((Customer cust, SearchInvoiceList inv) => cust.Id == inv.CustomerId)
                .FullJoin((PaymentStatus stat, SearchInvoiceList inv) => stat.Id == inv.PaymentStatusId)
                //.Where(inv => inv.Id == 1 && inv.Id.Equals(1)
                //|| inv.StatusId > 1 && inv.StatusId < 1
                //|| inv.StatusId <= 1 && inv.StatusId >= 1 && inv.Id != 1
                //|| inv.No == "IV" && inv.No.Equals("IV")
                //|| inv.No.Contains("IV") && inv.No.StartsWith("IV") && inv.No.EndsWith("IV")
                //|| !inv.No.Contains("IV") && !inv.No.StartsWith("IV") && !inv.No.EndsWith("IV")
                //|| inv.No.ToLower().Contains("IV".ToLower()) && inv.No.ToLower().StartsWith("IV".ToLower())
                //&& inv.No.ToLower().EndsWith("IV".ToLower())
                //|| !inv.No.ToLower().Contains("IV".ToLower()) && !inv.No.ToLower().StartsWith("IV".ToLower())
                //&& !inv.No.ToLower().EndsWith("IV".ToLower())
                //&& inv.Id.Equals(long.Parse("1"))
                //|| ids.Contains(inv.Id) && nos.Contains(inv.No) && inv.No == null
                //|| !ids.Contains(inv.Id) && !nos.Contains(inv.No))
                .Where(inv => inv.Id == 1 && inv.Id.Equals(1))
                .Or(inv => inv.StatusId > 1 && inv.StatusId < 1 && inv.StatusId <= 1 && inv.StatusId >= 1)
                .Or(inv => inv.Id != 1 && !inv.Id.Equals(1))
                .Or(inv => inv.No == "IV" && inv.No.Equals("IV"))
                .Or(inv => inv.No != "IV" && !inv.No.Equals("IV"))
                .Or(inv => inv.No == null && inv.No .Equals(null))
                .Or(inv => inv.No != null && !inv.No.Equals(null))
                .Or(inv => inv.No.Contains("IV") && inv.No.StartsWith("IV") && inv.No.EndsWith("IV"))
                .Or(inv => !inv.No.Contains("IV") && !inv.No.StartsWith("IV") && !inv.No.EndsWith("IV"))
                .Or(inv => inv.No.ToLower().Contains("IV") && inv.No.ToLower().StartsWith("IV")
                && inv.No.ToLower().EndsWith("IV"))
                .Or(inv => !inv.No.ToLower().Contains("IV") && !inv.No.ToLower().StartsWith("IV")
                && !inv.No.ToLower().EndsWith("IV"))
                .Or(inv => ids.Contains(inv.Id) && nos.Contains(inv.No))
                .Or(inv => !ids.Contains(inv.Id) && !nos.Contains(inv.No))

                .GroupBy(inv => inv.No)
                .OrderBy(inv => inv.Id)
                .OrderByDesc(inv => inv.No)
                .Select(inv => new Cashier { Id = inv.CashierId, Name = inv.CreatedBy })
                .Select(inv => new Customer { Id = inv.CustomerId, Name = inv.CustomerName })
                .Select(inv => new InvoiceStatus { Id = inv.StatusId, Name = inv.StatusName })
                .Select(inv => new PaymentStatus { Id = inv.PaymentStatusId, Name = inv.PaymentStatusName })
                .SqlProvider.FormatGet<SearchInvoiceList>().SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
