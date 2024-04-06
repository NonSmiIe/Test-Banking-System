using System.ComponentModel.DataAnnotations;

namespace BankingSolution.Models
{
    public class Account
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public decimal Balance { get; set; }
    }
}
