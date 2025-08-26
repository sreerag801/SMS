using SMS.API.API;
using SMS.API.Middlewares;
using SMS.Infrastructure;
using SMS.Service;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog(lc => lc.ReadFrom.Configuration(builder.Configuration));

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment.IsDevelopment());
    builder.Services.AddServicesDependencies(builder.Configuration, builder.Environment.IsDevelopment());

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Endpoints
    app.MapDefaultEndpoints();
    app.MapErrorLogEndpoints();
    app.MapStudentEndpoints();
    app.MapCourseEndpoints();

    // Custom middlewares.
    app.UseMiddleware<ExceptionMiddleware>();

    app.Run();
}
catch (Exception exception)
{
    Log.Error(exception, "Application terminated unexpectedly");
    Log.Error(exception.Message);
    Console.WriteLine("Application terminated unexpectedly");
}
finally
{
    Console.WriteLine("Application Shutdown");
    Log.CloseAndFlush();
}
