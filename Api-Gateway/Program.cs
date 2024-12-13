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

// ENABLE CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000")  // Allow only your frontend origin
            .AllowAnyHeader()  // Allow any header
            .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
            .AllowCredentials();  // Allow credentials (cookies, HTTP authentication)
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS policy before other middleware
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();