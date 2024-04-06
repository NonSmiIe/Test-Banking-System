using BankingSolution.Models;
using BankingSolution.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class TokenServiceTests
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData: new Dictionary<string, string>
                {
                    { "JwtSettings:SecretKey", "TestSecretKeyHereTestSecretKeyHereTestSecretKeyHere" },
                    { "JwtSettings:Issuer", "TestIssuerHere" },
                    { "JwtSettings:Audience", "TestAudienceHere" }
                })
                .Build();
        }

        [Test]
        public void CreateToken_Returns_Valid_JwtToken()
        {
            var tokenService = new TokenService(_configuration);
            var account = new Account { Username = "testuser" }; 

            var token = tokenService.CreateToken(account);

            Assert.IsNotNull(token);
            Assert.IsInstanceOf<string>(token);
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var parsedToken = tokenHandler.ReadJwtToken(token);


            Assert.That(account.Username, Is.EqualTo(parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value));
            Assert.That(_configuration.GetSection("JwtSettings:Issuer").Value, Is.EqualTo(parsedToken.Issuer));
            Assert.That(_configuration.GetSection("JwtSettings:Audience").Value, Is.EqualTo(parsedToken.Audiences.FirstOrDefault()));
        }
    }
}
