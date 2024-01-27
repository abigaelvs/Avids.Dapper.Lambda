using Npgsql;

using Avids.Dapper.Lambda.Core.SetC;
using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.Test
{
    public class UpdateTest
    {
        [Fact]
        public void TestUpdate()
        {
            string expected = @"UPDATE ""Invoice""  SET  ""No""=@UPDATE_No , "
                + @"""StatusId""=@UPDATE_StatusId , ""PaymentStatusId""=@UPDATE_PaymentStatusId , "
                + @"""CashierId""=@UPDATE_CashierId , ""UpdatedByUserId""=@UPDATE_UpdatedByUserId , "
                + @"""CustomerId""=@UPDATE_CustomerId , ""CreatedDate""=@UPDATE_CreatedDate , "
                + @"""UpdatedDate""=@UPDATE_UpdatedDate    WHERE ""Id"" = @Id";

            Invoice inv = new();
            inv.No = "IV123";
            inv.CustomerId = 1;
            inv.CashierId = 1;
            inv.StatusId = 1;
            inv.CreatedDate = DateTime.Now;
            inv.UpdatedByUserId = null;
            inv.UpdatedDate = null;

            string actual = new NpgsqlConnection().CommandSet<Invoice>().SqlProvider
                .FormatUpdate(inv).SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestUpdateWithMultipleKeyAttribute()
        {
            string expected = @"UPDATE ""CashierInvoice""  SET  ""BillingNo""=@UPDATE_BillingNo"
                + @"    WHERE ""InvoiceId"" = @InvoiceId AND ""BillingId"" = @BillingId";

            InvoiceBilling inv = new();
            inv.InvoiceId = 1;
            inv.BillingId = 1;
            inv.BillingNo = "BILL123";

            string actual = new NpgsqlConnection().CommandSet<CashierInvoice>().SqlProvider
                .FormatUpdate(inv).SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestUpdateWithWhere()
        {
            string expected = @"UPDATE ""Invoice""  SET  ""No""=@UPDATE_No , "
                    + @"""StatusId""=@UPDATE_StatusId , ""PaymentStatusId""=@UPDATE_PaymentStatusId , "
                    + @"""CashierId""=@UPDATE_CashierId , ""UpdatedByUserId""=@UPDATE_UpdatedByUserId , "
                    + @"""CustomerId""=@UPDATE_CustomerId , ""CreatedDate""=@UPDATE_CreatedDate , "
                    + @"""UpdatedDate""=@UPDATE_UpdatedDate   WHERE ""Id"" = @Id1";

            Invoice inv = new();
            inv.No = "IV123";
            inv.CustomerId = 1;
            inv.CashierId = 1;
            inv.StatusId = 1;
            inv.CreatedDate = DateTime.Now;
            inv.UpdatedByUserId = null;
            inv.UpdatedDate = null;

            string actual = (new NpgsqlConnection().CommandSet<Invoice>()
                .Where(inv => inv.Id == 1) as Command<Invoice>)
                .SqlProvider.FormatUpdate(inv).SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestUpdateSelect()
        {
            string expected = @"UPDATE ""Invoice""  SET  ""No""=@UPDATE_No , "
                    + @"""StatusId""=@UPDATE_StatusId , ""PaymentStatusId""=@UPDATE_PaymentStatusId , "
                    + @"""CashierId""=@UPDATE_CashierId , ""UpdatedByUserId""=@UPDATE_UpdatedByUserId , "
                    + @"""CustomerId""=@UPDATE_CustomerId , ""CreatedDate""=@UPDATE_CreatedDate , "
                    + @"""UpdatedDate""=@UPDATE_UpdatedDate   WHERE ""Id"" IN "
                    + @"(SELECT ""Id"" FROM ""Invoice"" WHERE ""Id"" = @Id1  FOR UPDATE SKIP LOCKED) "
                    + @"RETURNING *";

            Invoice inv = new();
            inv.No = "IV123";
            inv.CustomerId = 1;
            inv.CashierId = 1;
            inv.StatusId = 1;
            inv.CreatedDate = DateTime.Now;
            inv.UpdatedByUserId = null;
            inv.UpdatedDate = null;

            string actual = (new NpgsqlConnection().CommandSet<Invoice>()
                .Where(inv => inv.Id == 1) as Command<Invoice>)
                .SqlProvider.FormatUpdateSelect(inv).SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
