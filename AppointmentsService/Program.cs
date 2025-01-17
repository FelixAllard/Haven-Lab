using AppointmentsService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Get the environment variable for DB_HOST (defaults to "localhost" if not set)
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";  // Use "mysql" when inside Docker, "localhost" locally
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3310";

// Register DbContext with a dynamically updated connection string
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection")
            .Replace("${DB_HOST}", dbHost)   // Only replace the Server part with dbHost
            .Replace("${DB_PORT}", dbPort),  // Replace DB_PORT part with dbPort
        new MySqlServerVersion(new Version(8, 0, 29))
    ));



// Add controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allows Docker to connect to it directly
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5114);
});

var app = builder.Build();

// Add authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// Configure Swagger for Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map the controllers
app.MapControllers();

// Bind the app to 0.0.0.0 on port 5114 to allow external access in Docker
app.Urls.Add("http://0.0.0.0:5114");

app.Run();