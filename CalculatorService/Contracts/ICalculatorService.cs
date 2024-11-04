namespace CalculatorAPI.Contracts
{
    public interface ICalculatorService
    {
        Task<double> CalculateAsync(double a, double b, string operation);
    }
}