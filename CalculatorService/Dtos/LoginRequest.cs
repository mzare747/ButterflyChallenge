namespace CalculatorAPI.Dtos
{
    public sealed record LoginRequest
    (
        string Username,
        string Password
    );
}
