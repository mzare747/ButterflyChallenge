namespace CalculatorAPI.Contracts
{
    public interface IJwtService
    {
        string GenerateToken(string username);
    }
}
