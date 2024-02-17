using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avids.Dapper.Lambda.Sample.Entity
{
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string No { get; set; }
        public long StatusId { get; set; }
        public long PaymentStatusId { get; set; }
        public long CashierId { get; set; }
        public long? UpdatedByUserId { get; set; }
        public long CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
