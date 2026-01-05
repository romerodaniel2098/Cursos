using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCourses.Application.Dtos;

public record CreateCourseDto([Required] string Title);
public record UpdateCourseDto([Required] string Title);
public record CourseResponseDto(Guid Id, string Title, string Status, bool IsDeleted, int LessonCount, DateTime UpdatedAt);
public record CourseDetailDto(Guid Id, string Title, string Status, bool IsDeleted, DateTime CreatedAt, DateTime UpdatedAt);

public record CourseSearchQuery(string? Q, string? Status, int Page = 1, int PageSize = 10);
public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
