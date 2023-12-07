using Microsoft.AspNetCore.Mvc;

namespace Avids.Dapper.Lambda.Sample.Models
{
    public class SearchInvoiceRequestDto
    {

        public string No { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public long CreatedUserId { get; set; }
    }
}
