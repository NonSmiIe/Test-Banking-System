using BankingSolution.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BankingSolution.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationContext _context;
        public AccountService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Account> GetByAsync(Expression<Func<Account, bool>> filter = null) 
        {
            return await _context.Accounts.FirstOrDefaultAsync(filter);
        }

        public async Task<IEnumerable<Account>> GetAllByAsync(Expression<Func<Account, bool>> filter = null)
        {
            if(filter != null)
            {
                return await _context.Accounts.Where(filter).ToListAsync();
            }

            return await _context.Accounts.ToListAsync();
        }

        public async Task<bool> IsExistAsync(Expression<Func<Account, bool>> filter = null)
        {
            return await _context.Accounts.AnyAsync(filter);
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
        }

        public void UpdateRange(IEnumerable<Account> accounts)
        {
            _context.Accounts.UpdateRange(accounts);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Account acc) 
        {
            await _context.Accounts.AddAsync(acc);
        }
    }
}
