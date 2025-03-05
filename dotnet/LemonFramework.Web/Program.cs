using Autofac;
using Autofac.Extensions.DependencyInjection;
using LemonFramework.Extension.DynamicWebApi;
using LemonFramework.Extension.ServiceRegistered;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(c => c.AddPolicy("any", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.AddControllers().AddDynamicWebApi(); // 控制器自动生成
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options => {

// });
// builder.Services.ServiceRegister(); // 服务注册

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()); // Autofac容易替换
builder.Host.ConfigureContainer<ContainerBuilder>(builder => {
    builder.RegisterModule<AutofacModuleRegister>(); // 服务模块注册
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
