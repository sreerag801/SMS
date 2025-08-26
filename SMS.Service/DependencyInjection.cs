using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMS.Service.Services.Course;
using SMS.Service.Services.Students;

namespace SMS.Service;
public static class DependencyInjection
{
    public static IServiceCollection AddServicesDependencies(
    this IServiceCollection services,
    IConfiguration configuration,
    bool isDevelopment = false
    )
    {
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ICourseService, CourseService>();
        return services;
    }
}
