using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Allow requests from React development server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// bind and register MongoDbSettings
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

// Load settings from config, override with in-file password as requested
var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>() ?? new MongoDbSettings();

// Hardcoded password per request (not recommended for production)
mongoSettings.ConnectionString = "mongodb+srv://TodoAdmin:TodoAdmin123@todocluster.uhecxgp.mongodb.net/?retryWrites=true&w=majority";
mongoSettings.DatabaseName ??= "TodoDb";

var clientSettings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
var mongoClient = new MongoClient(clientSettings);

builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton(sp => mongoClient.GetDatabase(mongoSettings.DatabaseName ?? "admin"));

builder.Services.AddSingleton<ITodoRepository, TodoRepository>();

// register concrete and map interface to same singleton instance
builder.Services.AddSingleton<TodoService>();
builder.Services.AddSingleton<ITodoService>(sp => sp.GetRequiredService<TodoService>());

var app = builder.Build();

// startup ping to surface auth/DNS errors early
try
{
    var db = app.Services.GetRequiredService<IMongoDatabase>();
    db.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
    Console.WriteLine("MongoDB ping succeeded.");
}
catch (Exception ex)
{
    Console.WriteLine("MongoDB startup check failed: " + ex.Message);
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowReactDev");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

// Marker type so Integration tests using WebApplicationFactory<Program> can find the Program type.
public partial class Program { }