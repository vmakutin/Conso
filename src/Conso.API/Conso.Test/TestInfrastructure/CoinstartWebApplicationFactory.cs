using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Conso.Test.TestInfrastructure
{
    public class ConsoWebApplicationFactory<TProgram>
        where TProgram : class
    {
        private class DisposableWebApplicationsFactory : WebApplicationFactory<TProgram>
        {
            private bool _disposedValue;
            public List<Action> DisposeActions { get; init; } = null!;

            protected override void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        foreach (var action in DisposeActions)
                        {
                            action();
                        }
                    }
                }
                _disposedValue = true;
            }
        }

        protected Uri? BaseAddress { get; set; }
        protected string? IsolationId { get; set; }
        protected string? AppSettingsTestProject { get; set; }
        protected Action<IWebHostBuilder>? WebHostBuilder { get; set; }
        protected List<Action<IServiceCollection>> ArrangedData { get; set; } = new List<Action<IServiceCollection>>();
        protected List<Action<IServiceCollection>> MockDbContexts { get; set; } = new List<Action<IServiceCollection>>();
        protected List<Action<IServiceCollection>> TestServices { get; set; } = new List<Action<IServiceCollection>>();
        protected List<Action<IServiceCollection>> PostConfigure { get; set; } = new List<Action<IServiceCollection>>();
        protected List<Action<IServiceProvider>> DisposeActions { get; set; } = new List<Action<IServiceProvider>>();

        public static ConsoWebApplicationFactory<TProgram> NewTestServer(string isolationId, Uri baseAddress)
        {
            return new ConsoWebApplicationFactory<TProgram> { BaseAddress = baseAddress, IsolationId = isolationId };
        }

        public ConsoWebApplicationFactory<TProgram> ArrangeDate<TDbContext>(Action<TDbContext> dataFactory)
            where TDbContext : DbContext
        {
            ArrangedData.Add(serviceCollection =>
            {
                var serviceProvider = serviceCollection.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var dbContext = scopedServices.GetService<TDbContext>();

                    dbContext!.Database.EnsureCreated();

                    dataFactory.Invoke(dbContext);

                    dbContext.SaveChanges();
                }
            });

            return this;
        }

        public virtual ConsoWebApplicationFactory<TProgram> AddSqlliteInMemoryDb<TDbContext>()
            where TDbContext : DbContext
        {
            MockDbContexts.Add(serviceCollection =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                var descriptor = serviceCollection.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<TDbContext>));
                if (descriptor != null)
                {
                    serviceCollection.Remove(descriptor);
                }

                // Add a new registration for ApplicationDbContext with an in-memory database
                serviceCollection.AddDbContext<TDbContext>(options =>
                {
                    // Provide a unique name for your in-memory database
                    options.UseSqlite(connection);
                });

                DisposeActions.Add((_) => connection.Dispose());
            });

            return this;
        }

        public ConsoWebApplicationFactory<TProgram> AddTestService(Action<IServiceCollection> action)
        {
            TestServices.Add(action);
            return this;
        }

        public virtual ConsoWebApplicationFactory<TProgram> WithAppSettingsRelativeToProjectOf(string appSettingsPath, Type entryPointType)
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetAssembly(entryPointType)!.Location)!;
            AppSettingsTestProject = Path.Combine(directoryName, appSettingsPath);
            return this;
        }

        public virtual ConsoWebApplicationFactory<TProgram> WithDefaultHostBuilder(string environment = "Development")
        {
            string environment2 = environment;
            WebHostBuilder = delegate (IWebHostBuilder builder)
            {
                builder.UseEnvironment(environment2);
                builder.ConfigureServices(delegate (IServiceCollection serviceCollection)
                {
                    if (MockDbContexts.Count > 0)
                    {
                        foreach (Action<IServiceCollection> mockDbContext in MockDbContexts)
                        {
                            mockDbContext(serviceCollection);
                        }
                    }

                    foreach (Action<IServiceCollection> item in PostConfigure)
                    {
                        item(serviceCollection);
                    }
                });

                if (!string.IsNullOrWhiteSpace(AppSettingsTestProject))
                {
                    builder.ConfigureAppConfiguration(delegate (WebHostBuilderContext _, IConfigurationBuilder conf)
                    {
                        conf.Sources.Clear();
                        conf.AddJsonFile(AppSettingsTestProject);
                    });
                }

                builder.ConfigureTestServices(delegate (IServiceCollection serviceCollection)
                {
                    foreach (Action<IServiceCollection> testService in TestServices)
                    {
                        testService(serviceCollection);
                    }

                    if (MockDbContexts.Count > 0)
                    {
                        foreach (Action<IServiceCollection> arrangedDatum in ArrangedData)
                        {
                            arrangedDatum(serviceCollection);
                        }
                    }
                });
            };
            return this;
        }

        public WebApplicationFactory<TProgram> Build()
        {
            if (WebHostBuilder == null)
            {
                throw new ArgumentNullException("WebHostBuilder");
            }

            if (BaseAddress == null)
            {
                throw new ArgumentNullException("BaseAddress");
            }

            List<Action> list = new List<Action>();
            WebApplicationFactory<TProgram> fixture = new DisposableWebApplicationsFactory
            {
                DisposeActions = list
            }
                .WithWebHostBuilder(WebHostBuilder);

            list.AddRange(DisposeActions.Select((Func<Action<IServiceProvider>, Action>)((x) => delegate
            {
                x(fixture.Services);
            })));

            fixture.Server.BaseAddress = BaseAddress;

            Reset();

            return fixture;
        }

        protected void Reset()
        {
            BaseAddress = null;
        }
    }
}
