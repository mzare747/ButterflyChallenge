

using CalculatorAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CalculatorAPI.Tests
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public JwtServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();

            // Mock the configuration sections for "Jwt:Secret", "Jwt:Issuer", and "Jwt:Audience"
            _mockConfiguration.Setup(config => config["Jwt:Secret"]).Returns("************************************************************");
            _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("TestAudience");

            _jwtService = new JwtService(_mockConfiguration.Object);
        }

        [Fact]
        public void GenerateToken_MustReturnValidJwtToken()
        {
            // Arrange
            string username = "testuser";

            // Act
            string token = _jwtService.GenerateToken(username);

            // Assert
            Assert.NotNull(token);
            Assert.False(string.IsNullOrEmpty(token));

            // Validate the token structure and claims
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_mockConfiguration.Object["Jwt:Secret"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // Disable for test purposes only; normally, you would validate it
                ValidIssuer = _mockConfiguration.Object["Jwt:Issuer"],
                ValidAudience = _mockConfiguration.Object["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                // Check that the token contains the correct username
                var usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
                Assert.NotNull(usernameClaim);
                Assert.Equal(username, usernameClaim.Value);
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Token validation failed: {ex.Message}");
            }
        }
    }
}
