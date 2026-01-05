using Microsoft.EntityFrameworkCore;
using OnlineCourses.Application.Common.Interfaces;
using OnlineCourses.Application.Dtos;
using OnlineCourses.Domain.Courses;

namespace OnlineCourses.Application.Services;

public class CourseService
{
    private readonly IApplicationDbContext _context;

    public CourseService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CourseResponseDto>> SearchAsync(CourseSearchQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

        // ✅ Importante: incluye Lessons para poder contar sin problemas
        var dbQuery = _context.Courses
            .AsNoTracking()
            .Include(c => c.Lessons)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Q))
            dbQuery = dbQuery.Where(c => c.Title.Contains(query.Q));

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<CourseStatus>(query.Status, true, out var status))
            dbQuery = dbQuery.Where(c => c.Status == status);

        var totalCount = await dbQuery.CountAsync();

        var items = await dbQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Title,
                c.Status.ToString(),
                c.IsDeleted,
                c.Lessons.Count(l => !l.IsDeleted),
                c.UpdatedAt
            ))
            .ToListAsync();

        return new PagedResult<CourseResponseDto>(items, totalCount, page, pageSize);
    }

    public async Task<CourseDetailDto?> GetByIdAsync(Guid id)
    {
        // Track no necesario
        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return null;

        return new CourseDetailDto(
            course.Id, course.Title, course.Status.ToString(),
            course.IsDeleted, course.CreatedAt, course.UpdatedAt
        );
    }

    // ✅ Este es el endpoint requerido /summary (ahora sí)
    public async Task<CourseSummaryDto?> GetSummaryAsync(Guid id)
    {
        // Incluye lecciones para contar y sacar última modificación
        var course = await _context.Courses
            .AsNoTracking()
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course is null) return null;

        var totalLessons = course.Lessons.Count(l => !l.IsDeleted);

        // "Fecha de última modificación": max entre curso y sus lecciones
        var lastModified = course.UpdatedAt;
        var lastLessonUpdate = course.Lessons
            .Where(l => !l.IsDeleted)
            .Select(l => (DateTime?)l.UpdatedAt)
            .Max();

        if (lastLessonUpdate.HasValue && lastLessonUpdate.Value > lastModified)
            lastModified = lastLessonUpdate.Value;

        return new CourseSummaryDto(
            course.Id,
            course.Title,
            course.Status.ToString(),
            totalLessons,
            lastModified
        );
    }

    public async Task<Guid> CreateAsync(CreateCourseDto dto)
    {
        var course = new Course(dto.Title);
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return course.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCourseDto dto)
    {
        // ✅ Cargar trackeado. FindAsync respeta filtro global, perfecto.
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return false;

        // ✅ idempotente: si no cambia, no guardes
        if (string.Equals(course.Title, dto.Title, StringComparison.Ordinal))
            return true;

        course.Update(dto.Title);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return false;

        // ✅ idempotente: si ya está borrado, no intentes borrar otra vez
        if (course.IsDeleted) return true;

        course.Delete();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PublishAsync(Guid id)
    {
        var course = await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return false;

        // ✅ idempotente (evita updates sin cambios)
        if (course.Status == CourseStatus.Published) return true;

        course.Publish(); // valida "tiene lección activa"
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnpublishAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return false;

        // ✅ idempotente
        if (course.Status == CourseStatus.Draft) return true;

        course.Unpublish();
        await _context.SaveChangesAsync();
        return true;
    }
}
