using BankingSolution.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankingSolution.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey key;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:SecretKey").Value!));
        }

        public string CreateToken(Account account)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, account.Username)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(_configuration.GetSection("JwtSettings:Issuer").Value!,
                _configuration.GetSection("JwtSettings:Audience").Value!,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
