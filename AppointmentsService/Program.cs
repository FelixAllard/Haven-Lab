using AppointmentsService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register DbContext with MySQL connection string
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 29))));

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