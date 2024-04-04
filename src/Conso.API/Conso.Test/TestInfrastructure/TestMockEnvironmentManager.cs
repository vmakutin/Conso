using Conso.API;
using Conso.Providers;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;

namespace Conso.Test.TestInfrastructure
{
    public static class TestMockEnvironmentManager
    {
        public static HttpClient ArrangeSutAndGetClient(List<object> testData, string appSettings, int port)
        {
            var isolatedId = Guid.NewGuid().ToString();

            var apiUnderTest = ConsoWebApplicationFactory<Program>
                .NewTestServer(isolatedId, new Uri($"http://localhost:{port}"))
                .WithAppSettingsRelativeToProjectOf(Path.Combine("appSettings", appSettings), typeof(TestMockEnvironmentManager))
                //.InterceptAuthorization("test-audience", true,
                //    new string[] { "Conso-read", "Conso-write" }
                //)
                .AddSqlliteInMemoryDb<ConsoDbContext>()
                .AddTestService(services => services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>())
                .ArrangeDate<ConsoDbContext>(x => x.AddRange(testData))
                .WithDefaultHostBuilder()
                .Build();

            return apiUnderTest.CreateClient();

        }
    }
}
