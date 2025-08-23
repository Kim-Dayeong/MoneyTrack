using System.ComponentModel.DataAnnotations;
using MoneyTrack.Models;

namespace MoneyTrack.Dtos;

public class CategoryCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    public CategoryKind Kind { get; set; }

    public uint? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CategoryUpdateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    public CategoryKind Kind { get; set; }

    public uint? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CategoryResponseDto
{
    public uint CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public CategoryKind Kind { get; set; }
    public uint? ParentCategoryId { get; set; }
    public bool IsActive { get; set; }
}