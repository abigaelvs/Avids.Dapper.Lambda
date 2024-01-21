using Avids.Dapper.Lambda.Test.Entity;
using Npgsql;

namespace Avids.Dapper.Lambda.Test
{
    public class UpdateTest
    {
        [Fact]
        public void TestUpdate()
        {
            string expected = @"UPDATE ""Invoice""  SET  ""Id""=@UPDATE_Id , ""No""=@UPDATE_No , "
                + @"""StatusId""=@UPDATE_StatusId , ""PaymentStatusId""=@UPDATE_PaymentStatusId , "
                + @"""CashierId""=@UPDATE_CashierId , ""UpdatedByUserId""=@UPDATE_UpdatedByUserId , "
                + @"""CustomerId""=@UPDATE_CustomerId , ""CreatedDate""=@UPDATE_CreatedDate , "
                + @"""UpdatedDate""=@UPDATE_UpdatedDate";

            Invoice inv = new();
            inv.Id = 1;
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
        public void TestUpdateWithWhere()
        {
            string expected = @"UPDATE ""Invoice""  SET  ""Id""=@UPDATE_Id , ""No""=@UPDATE_No , "
                    + @"""StatusId""=@UPDATE_StatusId , ""PaymentStatusId""=@UPDATE_PaymentStatusId , "
                    + @"""CashierId""=@UPDATE_CashierId , ""UpdatedByUserId""=@UPDATE_UpdatedByUserId , "
                    + @"""CustomerId""=@UPDATE_CustomerId , ""CreatedDate""=@UPDATE_CreatedDate , "
                    + @"""UpdatedDate""=@UPDATE_UpdatedDate   WHERE ""Id"" = @Id1";

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
                .Where(inv => inv.Id == 1)
                .SqlProvider.FormatUpdate(inv).SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
