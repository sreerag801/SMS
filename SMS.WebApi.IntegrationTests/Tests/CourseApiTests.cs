using SMS.Service.Shared.Dto;
using SMS.WebApi.IntegrationTests.Fixtures;
using System.Net.Http.Json;

namespace SMS.WebApi.IntegrationTests.Tests;
public class CourseApiTests(WebAppEFFixture fixture) : IClassFixture<WebAppEFFixture>
{
    [Fact]
    public async Task Call_GetAllCourses_ReturnCourseList()
    {
        var result = await fixture.HttpSendAsyncWithBasicAuth(HttpMethod.Get, "/api/v1/course/get-all");

        Assert.True(result.IsSuccessStatusCode);
        var data = await result.Content.ReadFromJsonAsync<List<CourseDto>>();

        Assert.NotNull(data);
    }
}
