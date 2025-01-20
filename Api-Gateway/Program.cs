using System.Text.Json;
using Api_Gateway.Services;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// For Remote API calls
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ServiceProductController>();
builder.Services.AddTransient<ServiceProductController>();
builder.Services.AddTransient<ServiceOrderController>();
builder.Services.AddTransient<ServiceDraftOrderController>();
builder.Services.AddHttpClient<ServiceAuthController>();
builder.Services.AddTransient<ServiceAuthController>();
builder.Services.AddHttpClient<ServiceAppointmentsController>();
builder.Services.AddTransient<ServiceAppointmentsController>();
builder.Services.AddTransient<ServiceEmailApiController>();
builder.Services.AddTransient<ServicePromoController>();


// ENABLE CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostAndSpecificUrl", builder =>
    {
        builder
            .WithOrigins(
                "http://localhost:3000",     // Localhost for development
                "http://96.23.35.62:3000/" // Specific production URL
            )
            .AllowAnyHeader()     // Allow any headers
            .AllowAnyMethod()     // Allow any HTTP method
            .AllowCredentials();  // Allow credentials (cookies, HTTP authentication)
    });
});

//Allows Docker to connect to it directly
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5158);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS policy before other middleware
app.UseCors("AllowLocalhostAndSpecificUrl");


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Urls.Add("http://0.0.0.0:5158");
app.Run();