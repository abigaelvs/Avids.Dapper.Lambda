using System.ComponentModel.DataAnnotations.Schema;

namespace Avids.Dapper.Lambda.Sample.Entity
{
    [Table("Invoice")]
    public class SearchInvoiceList
    {
        public long Id { get; set; }
        public string No { get; set; }
        public long StatusId { get; set; }
        public string StatusName { get; set; }
        public long PaymentStatusId { get; set; }
        public string PaymentStatusName { get; set; }
        public long CashierId { get; set; }
        public long UpdatedByUserId { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
