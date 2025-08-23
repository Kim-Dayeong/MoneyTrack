using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrack.Data;
using MoneyTrack.Dtos;
using MoneyTrack.Models;

namespace MoneyTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(AppDbContext db) : ControllerBase
{
    // 목록 + 필터: /api/transaction?from=2025-08-01&to=2025-08-31&categoryId=2
    [HttpGet]
    public async Task<ActionResult<List<TransactionResponseDto>>> GetAll(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] uint? categoryId)
    {
        var q = db.Transactions.AsQueryable();

        if (from is not null)      q = q.Where(t => t.TransactionDate >= from);
        if (to   is not null)      q = q.Where(t => t.TransactionDate <= to);
        if (categoryId is not null)q = q.Where(t => t.CategoryId == categoryId);

        var list = await q
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.TransactionId)
            .Select(t => new TransactionResponseDto {
                TransactionId = t.TransactionId,
                CategoryId = t.CategoryId,
                Amount = t.Amount,
                Note = t.Note,
                TransactionDate = t.TransactionDate,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // 단건
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionResponseDto>> GetOne(uint id)
    {
        var t = await db.Transactions.FindAsync(id);
        if (t is null) return NotFound();

        return Ok(new TransactionResponseDto {
            TransactionId = t.TransactionId,
            CategoryId = t.CategoryId,
            Amount = t.Amount,
            Note = t.Note,
            TransactionDate = t.TransactionDate,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        });
    }

    // 생성
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransactionCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        // 카테고리 존재 확인
        var catExists = await db.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
        if (!catExists) return BadRequest("CategoryId not found.");

        var e = new Transaction
        {
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Note = dto.Note,
            TransactionDate = dto.TransactionDate
        };

        db.Transactions.Add(e);
        await db.SaveChangesAsync();

        // DB 기본값 반영 위해 다시 조회
        var saved = await db.Transactions.FindAsync(e.TransactionId);
        var res = new TransactionResponseDto {
            TransactionId = saved!.TransactionId,
            CategoryId = saved.CategoryId,
            Amount = saved.Amount,
            Note = saved.Note,
            TransactionDate = saved.TransactionDate,
            CreatedAt = saved.CreatedAt,
            UpdatedAt = saved.UpdatedAt
        };

        return CreatedAtAction(nameof(GetOne), new { id = res.TransactionId }, res);
    }

    // 수정
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(uint id, [FromBody] TransactionUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        // 전역 NoTracking 켜둔 환경에서도 동작하도록 AsTracking 사용
        var t = await db.Transactions.AsTracking().FirstOrDefaultAsync(x => x.TransactionId == id);
        if (t is null) return NotFound();

        // 카테고리 존재 확인
        var catExists = await db.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
        if (!catExists) return BadRequest("CategoryId not found.");

        t.CategoryId = dto.CategoryId;
        t.Amount = dto.Amount;
        t.Note = dto.Note;
        t.TransactionDate = dto.TransactionDate;

        await db.SaveChangesAsync();
        return NoContent(); // 204
    }

    // 삭제
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(uint id)
    {
        var t = await db.Transactions.FindAsync(id);
        if (t is null) return NotFound();

        db.Transactions.Remove(t);
        await db.SaveChangesAsync();
        return NoContent(); // 204
    }
}