using System;

namespace OnlineCourses.Domain.Courses;

public class Lesson
{
    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Constructor vacío para EF Core
    private Lesson() { }

    public Lesson(Guid courseId, string title, int order)
    {
        Id = Guid.NewGuid();
        CourseId = courseId;
        Title = title;
        Order = order;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void Update(string title, int order)
    {
        Title = title;
        Order = order;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
