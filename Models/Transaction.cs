using MoneyTrack.Models;

public class Transaction
{
    public uint TransactionId { get; set; }
    public uint CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;
}