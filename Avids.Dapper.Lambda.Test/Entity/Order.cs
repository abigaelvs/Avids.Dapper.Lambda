using System.ComponentModel.DataAnnotations.Schema;

namespace Avids.Dapper.Lambda.Test.Entity
{
    public class Order
    {
        public long InvoiceId { get; set; }
        public long OrderId { get; set; }

        public string OrderName { get; set; }

        public long CustomerId { get; set; }
    }
}
