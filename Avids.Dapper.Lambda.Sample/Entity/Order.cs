using System.ComponentModel.DataAnnotations.Schema;

namespace Avids.Dapper.Lambda.Sample.Entity
{
    [Table("order")]
    public class Order
    {
        [Column("order_id")]
        public long OrderId { get; set; }

        [Column("order_name")]
        public string OrderName { get; set; }

        [Column("customer_id")]
        public long CustomerId { get; set; }
    }
}
