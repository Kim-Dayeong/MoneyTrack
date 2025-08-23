using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MoneyTrack.Models;

// DB ENUM('INCOME','EXPENSE')를 C# enum으로 사용
public enum CategoryKind { INCOME, EXPENSE }

[Table("Category")] // 테이블명
public class Category
{
    [Key]
    [Column("CategoryId")]
    public uint CategoryId { get; set; }   // MySQL INT UNSIGNED -> C# uint

    [Required, MaxLength(100)]
    [Column("Name")]
    public string Name { get; set; } = null!;

    [Column("Kind")]
    public CategoryKind Kind { get; set; }  // 아래 DbContext에서 문자열 변환 설정

    [Column("ParentCategoryId")]
    public uint? ParentCategoryId { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; } = true; // TINYINT(1) -> bool (CS에 TreatTinyAsBoolean=true)

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }

    // 자기참조 관계 (선택)
    [ForeignKey(nameof(ParentCategoryId))]
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
}