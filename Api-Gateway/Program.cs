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
//For Remote API calls
builder.Services.AddHttpClient();


//--- Dependencies From us
builder.Services.AddHttpClient<ServiceProductController>(); // Makes the Https Client and Inject it in the class chosen
builder.Services.AddTransient<ServiceProductController>(); // Makes that an instance of ServiceProductController will be injected whenever nescessary

builder.Services.AddHttpClient<ServiceOrderController>(); // Makes the Https Client and Inject it in the class chosen 
builder.Services.AddTransient<ServiceOrderController>(); // Makes that an instance of ServiceOrderController will be injected whenever nescessary

builder.Services.AddHttpClient<ServiceAuthController>(); // Makes the Https Client and Inject it in the class chosen 
builder.Services.AddTransient<ServiceAuthController>(); // Makes that an instance of ServiceOrderController will be injected whenever nescessary


//---

// ENABLES CORES V V V 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin() // Allow requests from any origin
            .AllowAnyHeader() // Allow any header
            .AllowAnyMethod(); // Allow any HTTP method (GET, POST, etc.)
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS with the defined policy
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

