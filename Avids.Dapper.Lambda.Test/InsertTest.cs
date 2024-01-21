using Avids.Dapper.Lambda.Test.Entity;
using Npgsql;

namespace Avids.Dapper.Lambda.Test
{
    public class InsertTest
    {
        [Fact]
        public void TestInsertInto()
        {
            string expected = @"INSERT INTO ""Invoice"" (""Id"",""No"",""StatusId""," 
                + @"""PaymentStatusId"",""CashierId"",""UpdatedByUserId"",""CustomerId""," 
                + @"""CreatedDate"",""UpdatedDate"") VALUES (@Id,@No,@StatusId,@PaymentStatusId,"
                + @"@CashierId,@UpdatedByUserId,@CustomerId,@CreatedDate,@UpdatedDate)";

            Invoice inv = new();
            inv.Id = 1;
            inv.No = "IV123";
            inv.CustomerId = 1;
            inv.CashierId = 1;
            inv.StatusId = 1;
            inv.CreatedDate = DateTime.Now;
            inv.UpdatedByUserId = null;
            inv.UpdatedDate = null;

            string actual = new NpgsqlConnection().CommandSet<Invoice>()
                .SqlProvider.FormatInsert(inv).SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
