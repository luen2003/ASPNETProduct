using IdentityServer.Authentication;
using IdentityServer.Services;
using IdentityServer.Attributes;
using IdentityServer.Enums;

namespace IdentityServer.Controllers
{
    //[Route("api/[controller]")]

    [Route("IdentityServer/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly AuthService _authService;

        private readonly ApplicationUser _applicationUser;

        public AuthController(ILogger<AuthController> logger, AuthService authService, ApplicationUser applicationUser)
        {
            _logger = logger;
            _authService = authService;
            _applicationUser = applicationUser;
        }

        [HttpPost("authService")]
        public async Task<IActionResult> AuthService([FromBody] AuthServiceRequest param)
        {
            try
            {
                var response = await _authService.AuthCreateToken(param);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                _logger.LogDebug(ex.StackTrace);
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("TestAPI")]
        public async Task<IActionResult> GetDataTest()
        {
            try
            { 
                return Ok("Responsetest");
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                _logger.LogDebug(ex.StackTrace);
                return StatusCode(500, ex.Message);
            }

        }
    }
}
