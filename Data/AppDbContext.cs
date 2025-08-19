using Microsoft.EntityFrameworkCore;
using MoneyTrack.Models;

namespace MoneyTrack.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        var c = b.Entity<Category>();
        c.ToTable("Category");
        c.HasKey(x => x.CategoryId);

        // ENUM('INCOME','EXPENSE') <-> C# enum 간 문자열 변환
        c.Property(x => x.Kind).HasConversion<string>().HasMaxLength(7);

        // 자기참조 외래키 (ON DELETE SET NULL)
        c.HasOne(x => x.Parent)
         .WithMany(x => x.Children)
         .HasForeignKey(x => x.ParentCategoryId)
         .OnDelete(DeleteBehavior.SetNull);

        // 선택: 유용한 인덱스
        c.HasIndex(x => x.ParentCategoryId);
        c.HasIndex(x => x.Kind);
    }
}