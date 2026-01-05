using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineCourses.Domain.Courses;

public class Course
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public CourseStatus Status { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<Lesson> _lessons = new();
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    // Constructor vacío para EF Core
    private Course() { }

    public Course(string title)
    {
        Id = Guid.NewGuid();
        Title = title;
        Status = CourseStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void Update(string title)
    {
        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (!_lessons.Any(l => !l.IsDeleted))
        {
            throw new InvalidOperationException("Cannot publish a course without active lessons.");
        }
        Status = CourseStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        Status = CourseStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddLesson(string title, int order)
    {
        if (_lessons.Any(l => !l.IsDeleted && l.Order == order))
        {
             throw new InvalidOperationException("Order must be unique within the course.");
        }
        
        var lesson = new Lesson(Id, title, order);
        _lessons.Add(lesson);
        UpdatedAt = DateTime.UtcNow;
    }

    // Método helper para añadir una lección ya instanciada (útil si la lógica de creación está fuera)
    // Pero por agregate root, lo ideal es hacerlo aquí.
    // Vamos a permitir manipular la colección de lecciones con cuidado.
}
