using DotNetEnv;
using Email_Api.Database;
using Email_Api.Service;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5092);
});

Env.Load("../.env");

// Add services to the container.

// Get the environment variable for DB_HOST (defaults to "localhost" if not set)
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";  // Use "mysql" when inside Docker, "localhost" locally
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3310";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "user";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "pwd";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection")
            .Replace("${DB_HOST}", dbHost)  
            .Replace("${DB_PORT}", dbPort)
            .Replace("${DB_USER}", dbUser)
            .Replace("${DB_PASSWORD}", dbPassword)
            
        ,  
        new MySqlServerVersion(new Version(8, 0, 29))
    ));

//------------------------- DEPENDECY INJECTION
builder.Services.AddTransient<ISmtpConnection, SmtpConnection>();
builder.Services.AddSingleton<ITemplateManager,TemplateManager>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ISmtpClient, SmtpClient>();
builder.Services.AddTransient<ITemplateService, TemplateService>();
builder.Services.AddTransient<IEmailLogService, EmailLogService>();

//--------------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Urls.Add("http://0.0.0.0:5092");
app.Run();