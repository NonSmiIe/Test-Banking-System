using BankingSolution.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingSolution
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Account> Accounts => Set<Account>();
        public ApplicationContext()
        {
            Database.EnsureCreated();  
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
               .Property(p => p.Balance)
               .HasColumnType("money");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Data Source=bank.db");
        }
    }
}
