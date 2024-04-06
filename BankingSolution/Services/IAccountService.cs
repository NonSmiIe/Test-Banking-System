using BankingSolution.Models;
using System.Linq.Expressions;

namespace BankingSolution.Services
{
    public interface IAccountService
    {
        Task<Account> GetByAsync(Expression<Func<Account, bool>> filter = null);
        Task<IEnumerable<Account>> GetAllByAsync(Expression<Func<Account, bool>> filter = null);
        Task<bool> IsExistAsync(Expression<Func<Account, bool>> filter = null);
        Task SaveAsync();
        void Update(Account account);
        void UpdateRange(IEnumerable<Account> accounts);
        Task AddAsync(Account acc);
    }
}
