
using CalculatorAPI.Contracts;
using CalculatorAPI.Dtos;

namespace CalculatorAPI.Controllers
{
    [ApiVersion("2.0", Deprecated = false)]
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthController(IJwtService jwtService, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _configuration = configuration;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] LoginRequest request)
        {
            var validUsername = _configuration["DummyCredentials:Username"];
            var validPassword = _configuration["DummyCredentials:Password"];

            // Validate user credentials (for demonstration, using hardcoded validation)
            if (request.Username != validUsername || request.Password != validPassword)
            {
                return Unauthorized("Invalid credentials");
            }

            // Generate the JWT token using the JwtService
            var token = _jwtService.GenerateToken(request.Username);

            return Ok(new { Token = token });
        }
    }
}
