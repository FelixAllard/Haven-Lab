using DotNetEnv;
using Newtonsoft.Json.Serialization;
using Shopify_Api;
using Shopify_Api.Controllers;
using ShopifySharp;
using ShopifySharp.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);



Env.Load("../.env");
// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var shopifyConfig = builder.Configuration.GetSection("ShopifyApiCredentials");
builder.Services.AddSingleton(sp =>
    new Shopify_Api.ShopifyRestApiCredentials(
        shopUrl: Environment.GetEnvironmentVariable("SHOPIFY_API_SHOP_URL")?? shopifyConfig["ShopUrl"],
        accessToken: Environment.GetEnvironmentVariable("SHOPIFY_API_ACCESS_TOKEN")??shopifyConfig["AccessToken"]
    )
);
//--- Dependencies From us
builder.Services.AddTransient<ProductValidator>(); // Makes that an instance of ProductValidator will be injected whenever nescessary



//---
    
builder.Services.AddShopifySharp<LeakyBucketExecutionPolicy>();


//Allows Docker to connect to it directly
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5106);
});

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
app.Urls.Add("http://0.0.0.0:5106");
app.Run();