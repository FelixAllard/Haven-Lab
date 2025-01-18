using Email_Api.Service;
using MailKit.Net.Smtp;

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

//------------------------- DEPENDECY INJECTION
builder.Services.AddTransient<ISmtpConnection, SmtpConnection>();
builder.Services.AddSingleton<ITemplateManager,TemplateManager>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ISmtpClient, SmtpClient>();

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