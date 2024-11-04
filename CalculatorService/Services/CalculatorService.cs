using CalculatorAPI.Contracts;

namespace CalculatorAPI.Services
{
    internal class CalculatorService : ICalculatorService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CalculatorService> _logger;

        public CalculatorService(IDistributedCache cache, ILogger<CalculatorService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<double> CalculateAsync(double a, double b, string operation)
        {
            string cacheKey = $"{a}_{operation}_{b}";
            string cachedResult = await _cache.GetStringAsync(cacheKey) ?? string.Empty;

            if (!string.IsNullOrEmpty(cachedResult))
            {
                _logger.LogInformation($"Cache hit for operation: {a} {operation} {b}");
                return double.Parse(cachedResult);
            }

            double result = operation switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                "/" => b != 0 ? a / b : throw new DivideByZeroException("Division by zero is not allowed"),
                _ => throw new InvalidOperationException("Invalid operation")
            };

            _logger.LogInformation($"Calculated result for {a} {operation} {b} : {result}");

            await _cache.SetStringAsync(cacheKey, result.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),

            });

            return result;
        }
    }
}