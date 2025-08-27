using Microsoft.AspNetCore.Mvc;
using ServiceAPI.Models.TransactionRequest;
using ServiceAPI.Models.TransactionResponse;
using ServiceAPI.Utils;

namespace ServiceAPI.Controllers
{
    [Route("ServiceAPI/v1/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {

        private readonly ServiceAPI.Services.TransactionService _service;
        public TransactionController(ServiceAPI.Services.TransactionService service)
        {
            _service = service;
        }

        [HttpPost("GetTransactionbyCommand")]
        public async Task<IActionResult> GetTransactionbyCommand([FromBody] CommmandRequestGoFirt TransactionRequest)
        {
            try
            {
                var response = await _service.GetDataByCommandCode(TransactionRequest); 
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }

        [HttpPost("SaveTransaction")]
        public async Task<IActionResult> SaveTransaction([FromBody] Models.TransactionResponse.CommmandRequestTo commmandRequest)
        {
            try
            { 
                var response = await _service.SaveTransactionbylistDataWH(commmandRequest);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }

        [HttpPost("SaveTransactionDOP1")]
        public async Task<IActionResult> SaveTransactionDOP1([FromBody] Models.TransactionResponse.CommmandRequestTo commmandRequest)
        {
            try
            {
                var response = await _service.SaveTransactionDOPToDOP1(commmandRequest);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }

    }
}
