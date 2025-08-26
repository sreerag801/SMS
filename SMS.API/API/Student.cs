using Microsoft.AspNetCore.Mvc;
using SMS.Service.Services.Students;
using SMS.Service.Shared.Requests;

namespace SMS.API.API;

public static class Student
{
    public static void MapStudentEndpoints(this WebApplication app)
    {
        var v1 = app.MapGroup("/api/v1/student").WithTags(nameof(Student).ToLowerInvariant()).WithOpenApi();

        v1.MapGet("", async (IStudentService service, CancellationToken cancellationToken) =>
        {
            return await service.GetAllAsync(cancellationToken);
        })
        .WithName("GetAllStudents");

        v1.MapGet("/{studentId}", async ([FromRoute] Guid studentId, IStudentService service, CancellationToken cancellationToken) =>
        {
            return await service.GetByIdAsync(studentId, cancellationToken);
        })
        .WithName("GetStudentDetilsById");

        v1.MapPost("/create", async ([FromBody] CreateStudentRequest request, IStudentService service, CancellationToken cancellationToken) =>
        {
            return await service.CreateAsync(request, cancellationToken);
        })
        .WithName("CreateStudent");

        v1.MapPut("/update/{studentId}", async ([FromRoute]Guid studentId,[FromBody] CreateStudentRequest request, IStudentService service, CancellationToken cancellationToken) =>
        {
            return await service.UpdateAsync(studentId, request, cancellationToken);
        })
        .WithName("UpdateStudent");

        v1.MapDelete("/delete/{studentId}", async ([FromRoute] Guid studentId, IStudentService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteByIdAsync(studentId, cancellationToken);
            return Results.Ok($"Successfully removed student {studentId}.");
        })
        .WithName("DeleteStudent");
    }
}
