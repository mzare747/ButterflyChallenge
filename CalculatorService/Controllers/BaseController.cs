
namespace CalculatorAPI.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }
    }
}
