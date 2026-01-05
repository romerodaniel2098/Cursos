using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCourses.Application.Dtos;

public record CreateLessonDto([Required] Guid CourseId, [Required] string Title, [Required] int Order);
public record UpdateLessonDto([Required] string Title, [Required] int Order);
public record LessonResponseDto(Guid Id, Guid CourseId, string Title, int Order, bool IsDeleted);
