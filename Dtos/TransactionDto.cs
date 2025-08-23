using System.ComponentModel.DataAnnotations;

namespace MoneyTrack.Dtos;

public class TransactionCreateDto
{
    [Required] public uint CategoryId { get; set; }
    [Required] public decimal Amount { get; set; }
    [MaxLength(255)] public string? Note { get; set; }
    [Required] public DateTime TransactionDate { get; set; } // "2025-08-23" 형태
}

public class TransactionUpdateDto
{
    [Required] public uint CategoryId { get; set; }
    [Required] public decimal Amount { get; set; }
    [MaxLength(255)] public string? Note { get; set; }
    [Required] public DateTime TransactionDate { get; set; }
}

public class TransactionResponseDto
{
    public uint TransactionId { get; set; }
    public uint CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}