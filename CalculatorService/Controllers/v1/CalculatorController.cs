
using CalculatorAPI.Contracts;

namespace CalculatorAPI.Controllers.V1
{
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class CalculatorController : BaseController
    {
        private readonly ICalculatorService _calculatorService;

        public CalculatorController(ILogger<CalculatorController> logger, ICalculatorService calculatorService)
            : base(logger)
        {
            _calculatorService = calculatorService;
        }

        [HttpGet("add")]
        public async Task<IActionResult> Add(double a, double b)
        {
            var result = await _calculatorService.CalculateAsync(a, b, "+");
            return Ok(result);
        }

        [HttpGet("subtract")]
        public async Task<IActionResult> Subtract(double a, double b)
        {
            var result = await _calculatorService.CalculateAsync(a, b, "-");
            return Ok(result);
        }

        [HttpGet("multiply")]
        public async Task<IActionResult> Multiply(double a, double b)
        {
            var result = await _calculatorService.CalculateAsync(a, b, "*");
            return Ok(result);
        }

        [HttpGet("divide")]
        public async Task<IActionResult> Divide(double a, double b)
        {
            if (b == 0) return BadRequest("Division by zero is not allowed.");
            var result = await _calculatorService.CalculateAsync(a, b, "/");
            return Ok(result);
        }
    }
}