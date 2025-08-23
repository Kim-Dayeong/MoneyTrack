using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyTrack.Data;
using MoneyTrack.Models;

namespace MoneyTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoryController(AppDbContext db)
    {
        _db = db;
    }

    // 전체 목록 조회
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Categories
            .OrderBy(c => c.ParentCategoryId)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return Ok(list);
    }

    // 단건 조회
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(uint id)
    {
        var found = await _db.Categories.FindAsync(id);
        if (found is null) return NotFound();
        return Ok(found);
    }

    // 생성
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Category input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
            return BadRequest("Name is required.");

        // 부모 카테고리 확인
        if (input.ParentCategoryId is uint pid &&
            !await _db.Categories.AnyAsync(c => c.CategoryId == pid))
            return BadRequest("ParentCategoryId not found.");

        _db.Categories.Add(input);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOne), new { id = input.CategoryId }, input);
    }

    // 수정
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(uint id, [FromBody] Category input)
    {
        if (id != input.CategoryId) return BadRequest("Id mismatch.");

        var found = await _db.Categories.FindAsync(id);
        if (found is null) return NotFound();

        // 필요한 필드 갱신
        found.Name = input.Name;
        found.Kind = input.Kind;
        found.ParentCategoryId = input.ParentCategoryId;
        found.IsActive = input.IsActive;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // 삭제
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(uint id)
    {
        var found = await _db.Categories.FindAsync(id);
        if (found is null) return NotFound();

        _db.Categories.Remove(found);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}