using Microsoft.AspNetCore.Mvc;
using ServiceAPI.Models.MD;
using ServiceAPI.Models.TransactionRequest;
using ServiceAPI.Services;

namespace ServiceAPI.Controllers
{
    [Route("ServiceAPI/v1/[controller]")]
    [ApiController]
    public class VehicleController : Controller
    {
        private readonly VehicleService _service;
        public VehicleController(VehicleService service)
        {
            _service = service;
        }

        [HttpGet("GetAllVehicle")]
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

        [HttpPost("GetDatabyVehicleCode")]
        public async Task<IActionResult> GetDatabyVehicleCode([FromBody] VehicleRequest vehicleRequest)
        {
            try
            {
                var response = await _service.GetDatabyVehicleCode(vehicleRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }

        [HttpPost("SaveVehicle")]
        public async Task<IActionResult> SaveVehicle([FromBody] VehicleSaveRequest request)
        {
            try
            {
                var response = await _service.SaveVehicleNew(request); 
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + ex.StackTrace);
            }
        }
    }
}
