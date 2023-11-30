using Avids.Dapper.Lambda.Sample.Entity;
using Avids.Dapper.Lambda.Sample.Services;
using Microsoft.AspNetCore.Mvc;

namespace Avids.Dapper.Lambda.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("customer")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _customerService.GetAll());
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetById(long customerId)
        {
            return Ok(await _customerService.GetById(customerId));
        }

        [HttpPost("customer")]
        public async Task<IActionResult> Insert([FromBody] Customer customer)
        {
            return Ok(await _customerService.Insert(customer));
        }

        [HttpPut("customer")]
        public async Task<IActionResult> Update([FromBody] Customer customer)
        {
            return Ok(await _customerService.Update(customer));
        }

        [HttpDelete("customer/{customerId}")]
        public async Task<IActionResult> Delete(long customerId)
        {
            return Ok(await _customerService.Delete(customerId));
        }
    }
}
