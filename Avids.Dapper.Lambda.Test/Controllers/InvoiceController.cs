using Microsoft.AspNetCore.Mvc;

using Avids.Dapper.Lambda.Test.Entity;
using Avids.Dapper.Lambda.Test.Services;

namespace Avids.Dapper.Lambda.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("invoice")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _invoiceService.GetAll());
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetById(long invoiceId)
        {
            return Ok(await _invoiceService.GetById(invoiceId));
        }

        [HttpPost("invoice")]
        public async Task<IActionResult> Insert([FromBody] Invoice invoice)
        {
            return Ok(await _invoiceService.Insert(invoice));
        }

        [HttpPut("invoice")]
        public async Task<IActionResult> Update([FromBody] Invoice invoice)
        {
            return Ok(await _invoiceService.Update(invoice));
        }

        [HttpDelete("invoice/{invoiceId}")]
        public async Task<IActionResult> Delete(long invoiceId)
        {
            return Ok(await _invoiceService.Delete(invoiceId));
        }
    }
}
