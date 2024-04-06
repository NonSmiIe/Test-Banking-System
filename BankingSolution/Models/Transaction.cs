namespace BankingSolution.Models
{
    public class Transaction
    {
        public int SenderId { get; set; }
        public decimal Amount { get; set; }
        public int RecipientId { get; set; }
    }
}
