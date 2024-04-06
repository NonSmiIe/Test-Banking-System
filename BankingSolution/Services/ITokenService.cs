using BankingSolution.Models;

namespace BankingSolution.Services
{
    public interface ITokenService
    {
        public string CreateToken(Account account);
    }
}
