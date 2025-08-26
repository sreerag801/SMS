using Microsoft.AspNetCore.Mvc;
using SMS.Service.Services.Course;
using SMS.Service.Shared.Dto;

namespace SMS.API.API;

public static class Course
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        var v1 = app.MapGroup("/api/v1/course").WithTags(nameof(Course).ToLowerInvariant()).WithOpenApi();

        v1.MapGet("/get-all", async (ICourseService service, CancellationToken cancellationToken) =>
        {
            return await service.GetAllCoursesAsync(cancellationToken);
        })
        .WithName("GetAllCourses");

        v1.MapGet("/{courseId}", async ([FromRoute] Guid courseId, ICourseService service, CancellationToken cancellationToken) =>
        {
            return await service.GetByIdAsync(courseId, cancellationToken);
        })
        .WithName("GetCourseById");

        v1.MapPost("/create", async ([FromBody] CourseDto request, ICourseService service, CancellationToken cancellationToken) =>
        {
            return await service.CreateAsync(request, cancellationToken);
        })
        .WithName("CreateNewCourse");

        v1.MapDelete("/{courseId:guid}", async (Guid courseId, ICourseService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(courseId, cancellationToken);
            return Results.Ok($"Deleted {courseId}");
        })
        .WithName("DeleteCourse");

        v1.MapPut("/{courseId:guid}", async (Guid courseId, [FromBody]CourseDto request, ICourseService service, CancellationToken cancellationToken) =>
        {
            return await service.UpdateAsync(courseId, request, cancellationToken);
        })
        .WithName("UpdateCourse");
    }
}
