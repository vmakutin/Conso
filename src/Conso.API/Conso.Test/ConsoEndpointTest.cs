using Conso.API.Gates.Rest.Dto;
using Conso.Providers.Entities;
using Conso.Test.TestInfrastructure;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Conso.Test
{
    public class ConsoEndpointTest
    {
        private const int Port = 5001;
        private const string appSettings = "appsettings.Integration.json";

        [Fact]
        public async void Test2()
        {
            var httpClient = TestMockEnvironmentManager.ArrangeSutAndGetClient(TestData, appSettings, Port);

            var response = await httpClient
                .GetAsync("/WeatherForecast");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var dtos = JsonSerializer.Deserialize<List<ClassDto>>(content, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});

            dtos.Should().BeEquivalentTo(ExpectedData);
        }

        private List<object> ExpectedData =>
            new List<object>()
            {
                new ClassDto()
                {
                    Id = Guid.Parse("8E348A42-9272-4002-9154-8E6741001B90"),
                    Name = "A"
                },
                new ClassDto()
                {
                    Id = Guid.Parse("8E348A42-9272-4002-9154-8E6741001B91"),
                    Name = "B"
                },
            };

        private List<object> TestData =>
            new List<object>()
            {
                new ClassEntity()
                {
                    Id = Guid.Parse("8E348A42-9272-4002-9154-8E6741001B90"),
                    Name = "A"
                },
                new ClassEntity()
                {
                    Id = Guid.Parse("8E348A42-9272-4002-9154-8E6741001B91"),
                    Name = "B"
                },
            } ;
    }
}
