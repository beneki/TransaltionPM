using TranslationProjectManagement.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
var currentDir = Directory.GetCurrentDirectory();
var envFilePath = currentDir;

// Move up two levels to get to the parent directory
for (int i = 0; i < 3; i++)
{
    envFilePath = Directory.GetParent(envFilePath).FullName;
}

envFilePath = Path.Combine(envFilePath, ".env");
if (File.Exists(envFilePath))
{
    var envVars = File.ReadAllLines(envFilePath);
    foreach (var envVar in envVars)
    {
        var keyValue = envVar.Split('=');
        if (keyValue.Length == 2)
        {
            Environment.SetEnvironmentVariable(keyValue[0], keyValue[1]);
        }
    }
}

// Add configuration for appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Replace placeholders with environment variables
var dbUsername = Environment.GetEnvironmentVariable("DB_USERNAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");

builder.Configuration["ConnectionStrings:DefaultConnection"] = builder!.Configuration["ConnectionStrings:DefaultConnection"]!
        .Replace("{DB_USERNAME}", dbUsername)
        .Replace("{DB_PASSWORD}", dbPassword)
        .Replace("{DB_NAME}", dbName)
        .Replace("{DB_PORT}", dbPort);

// Add services to the container.
builder.Services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")  // Change this to your frontend URL
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Use CORS
app.UseCors("AllowSpecificOrigin");

app.MapControllers();
app.Run();
