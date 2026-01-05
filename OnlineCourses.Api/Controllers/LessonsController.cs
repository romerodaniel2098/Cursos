using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Application.Dtos;
using OnlineCourses.Application.Services;

namespace OnlineCourses.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/lessons")]
public class LessonsController : ControllerBase
{
    private readonly LessonService _lessonService;

    public LessonsController(LessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourse(Guid courseId)
    {
        var lessons = await _lessonService.GetByCourseIdAsync(courseId);
        return Ok(lessons);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLessonDto dto)
    {
        try
        {
            var id = await _lessonService.CreateAsync(dto);
            return Ok(new { id });
        }
        catch (Exception ex) 
        { 
            return BadRequest(new { message = ex.Message }); 
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateLessonDto dto)
    {
        try
        {
            var ok = await _lessonService.UpdateAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _lessonService.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpPatch("{id}/reorder")]
    public async Task<IActionResult> Reorder(Guid id, [FromQuery] string direction)
    {
        if (direction != "up" && direction != "down") return BadRequest("Direction must be 'up' or 'down'");
        
        var ok = await _lessonService.ReorderAsync(id, direction);
        return ok ? NoContent() : BadRequest("Cannot move in that direction");
    }
}
