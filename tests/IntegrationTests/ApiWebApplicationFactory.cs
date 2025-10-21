using Infrastructure.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace IntegrationTests
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing MongoDbSettings and IMongoDatabase registrations if present
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(MongoDbSettings));
            if (descriptor != null)
                services.Remove(descriptor);

            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IMongoDatabase));
            if (dbDescriptor != null)
                services.Remove(dbDescriptor);

            // Register test settings and test database
            var testSettings = new MongoDbSettings
            {
                ConnectionString = "mongodb+srv://TodoAdmin:TodoAdmin123@todocluster.uhecxgp.mongodb.net/?retryWrites=true&w=majority",
                DatabaseName = "TodoDbTest"
            };
            var client = new MongoClient(testSettings.ConnectionString);
            var database = client.GetDatabase(testSettings.DatabaseName);

            services.AddSingleton(testSettings);
            services.AddSingleton(database);
        });
    }
}
}