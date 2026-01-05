using OnlineCourses.Domain.Courses;
using Xunit;

namespace OnlineCourses.Tests;

public class MandatoryDomainTests
{
    [Fact]
    public void PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var course = new Course("Test Course");
        course.AddLesson("Lesson 1", 1); // Mock adding lesson

        // Act
        course.Publish();

        // Assert
        Assert.Equal(CourseStatus.Published, course.Status);
    }

    [Fact]
    public void PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var course = new Course("Empty Course");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => course.Publish());
    }

    [Fact]
    public void CreateLesson_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var course = new Course("Course");
        
        // Act
        course.AddLesson("Lesson 1", 1);
        course.AddLesson("Lesson 2", 2);

        // Assert
        Assert.Equal(2, course.Lessons.Count);
    }

    [Fact]
    public void CreateLesson_WithDuplicateOrder_ShouldFail()
    {
        // Arrange
        var course = new Course("Course");
        course.AddLesson("Lesson 1", 1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => course.AddLesson("Lesson 2", 1));
    }

    [Fact]
    public void DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var course = new Course("Course to delete");

        // Act
        course.Delete();

        // Assert
        Assert.True(course.IsDeleted);
    }
}
