using Microsoft.EntityFrameworkCore;
using OnlineCourses.Domain.Courses;

namespace OnlineCourses.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Lesson> Lessons { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
