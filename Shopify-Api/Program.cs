using Shopify_Api;
using Shopify_Api.Controllers;
using ShopifySharp;
using ShopifySharp.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


DependencyInjection(builder.Services);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(sp =>
    new Shopify_Api.ShopifyRestApiCredentials(
        shopUrl: "vc-shopz.myshopify.com",
        accessToken: "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"
    )
);
    
    
builder.Services.AddShopifySharp<LeakyBucketExecutionPolicy>();


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

app.Run();
//makes the nescessary dependency injection
void DependencyInjection(IServiceCollection services)
{
    
    
}