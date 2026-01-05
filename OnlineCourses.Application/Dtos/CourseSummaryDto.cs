namespace OnlineCourses.Application.Dtos;

public record CourseSummaryDto(
    Guid Id,
    string Title,
    string Status,
    int TotalLessons,
    DateTime LastModifiedAt
);
