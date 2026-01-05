using Microsoft.EntityFrameworkCore;
using OnlineCourses.Application.Common.Interfaces;
using OnlineCourses.Application.Dtos;
using OnlineCourses.Domain.Courses;

namespace OnlineCourses.Application.Services;

public class LessonService
{
    private readonly IApplicationDbContext _context;

    public LessonService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LessonResponseDto>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Lessons
            .AsNoTracking()
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .Select(l => new LessonResponseDto(l.Id, l.CourseId, l.Title, l.Order, l.IsDeleted))
            .ToListAsync();
    }

    public async Task<Guid> CreateAsync(CreateLessonDto dto)
    {
        // ✅ Valida que el curso exista (y no esté borrado por filtro global)
        // Note: AnyAsync might check deleted courses if QueryFilter is disabled, but here it is enabled by default.
        // However, Domain rules usually imply adding to active courses. 
        // We will assume the global filter handles IsDeleted.
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == dto.CourseId);
        if (!courseExists) throw new Exception("Course not found");

        // ✅ Validar orden único (filtro global ya excluye IsDeleted=true)
        var exists = await _context.Lessons.AnyAsync(l => l.CourseId == dto.CourseId && l.Order == dto.Order);
        if (exists) throw new Exception("Order must be unique within the course.");

        // ✅ Crear directamente la Lesson (más simple y controlado)
        var lesson = new Lesson(dto.CourseId, dto.Title, dto.Order); 
        _context.Lessons.Add(lesson);

        await _context.SaveChangesAsync();
        return lesson.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateLessonDto dto)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null) return false;

        // ✅ idempotencia básica: si nada cambia, no guardes
        if (lesson.Title == dto.Title && lesson.Order == dto.Order) return true;

        if (lesson.Order != dto.Order)
        {
            var exists = await _context.Lessons.AnyAsync(l =>
                l.CourseId == lesson.CourseId &&
                l.Order == dto.Order &&
                l.Id != id);

            if (exists) throw new Exception("Order must be unique within the course.");
        }

        lesson.Update(dto.Title, dto.Order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null) return false;

        if (lesson.IsDeleted) return true;

        lesson.Delete();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderAsync(Guid id, string direction)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null) return false;

        // Buscar lección adyacente
        var lessons = await _context.Lessons
            .Where(l => l.CourseId == lesson.CourseId && !l.IsDeleted)
            .OrderBy(l => l.Order)
            .ToListAsync();

        var index = lessons.FindIndex(l => l.Id == id);
        if (index == -1) return false;

        Lesson? other = null;
        if (direction == "up" && index > 0) other = lessons[index - 1];
        else if (direction == "down" && index < lessons.Count - 1) other = lessons[index + 1];

        if (other != null)
        {
            // Swap orders
            var tempOrder = lesson.Order;
            lesson.Update(lesson.Title, other.Order);
            other.Update(other.Title, tempOrder);
            
            await _context.SaveChangesAsync();
            return true;
        }

        return false; // No movement possible
    }
}
