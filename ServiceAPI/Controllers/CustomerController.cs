using Microsoft.AspNetCore.Mvc;
using ServiceAPI.Models.MD;
using ServiceAPI.Models.TransactionRequest;
using ServiceAPI.Services;

namespace ServiceAPI.Controllers
{
    [Route("ServiceAPI/v1/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly CustomerService _service;
        public CustomerController(CustomerService service)
        {
            _service = service;
        }

        [HttpGet("GetAllData")]
        public async Task<IActionResult> GetAllData()
        {
            try
            {
                var response = await _service.GetAllData();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }

        [HttpPost("GetDatabyCustomerCode")]
        public async Task<IActionResult> GetDatabyCustomerCode([FromBody] CustomerRequest customerRequest)
        {
            try
            {
                var response = await _service.GetDatabyCustomerCode(customerRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }
    }
}
