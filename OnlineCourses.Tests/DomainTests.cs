using OnlineCourses.Domain.Courses;

namespace OnlineCourses.Tests;

public class DomainTests
{
    [Fact]
    public void PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var course = new Course("Test Course");
        course.AddLesson("Lesson 1", 1);

        // Act
        course.Publish();

        // Assert
        Assert.Equal(CourseStatus.Published, course.Status);
    }

    [Fact]
    public void PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var course = new Course("Test Course");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => course.Publish());
        Assert.Equal("Cannot publish a course without active lessons.", exception.Message);
    }

    [Fact]
    public void CreateLesson_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var course = new Course("Test Course");
        course.AddLesson("Lesson 1", 1);

        // Act
        course.AddLesson("Lesson 2", 2);

        // Assert
        Assert.Equal(2, course.Lessons.Count);
    }

    [Fact]
    public void CreateLesson_WithDuplicateOrder_ShouldFail()
    {
        // Arrange
        var course = new Course("Test Course");
        course.AddLesson("Lesson 1", 1);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => course.AddLesson("Lesson 2", 1));
        Assert.Equal("Order must be unique within the course.", exception.Message);
    }

    [Fact]
    public void DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var course = new Course("Test Course");

        // Act
        course.Delete();

        // Assert
        Assert.True(course.IsDeleted);
    }
}
