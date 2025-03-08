using Autofac;
using Autofac.Extensions.DependencyInjection;
using LemonFramework.Common.Helper;
using LemonFramework.Core;
using LemonFramework.Extension.DynamicWebApi;
using LemonFramework.Extension.ServiceRegistered;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(c => c.AddPolicy("any", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.AddControllers().AddDynamicWebApi(); // 控制器自动生成
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options => {

// });
builder.Services.AddDbContext<LemonFrameworkDBContext>(options => {
    options.UseSqlServer(Appsettings.app(new string[] {"ConnectionStrings", "Default"}));
});
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()); // Autofac容易替换
builder.Host.ConfigureContainer<ContainerBuilder>(builder => {
    builder.RegisterModule<AutofacModuleRegister>(); // 服务模块注册
});


var app = builder.Build();
app.UseCors("any");
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();
