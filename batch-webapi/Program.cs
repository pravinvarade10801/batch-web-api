using batch_webapi.Data.Services;
using batch_webapi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Hosting;
using batch_webapi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FluentValidation.AspNetCore;
using System.Reflection;
using Azure.Storage.Blobs;
using System.IO;

using Microsoft.OpenApi.Models;
using System.Linq;
using Serilog.Sinks.MSSqlServer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

//Log.Logger = new LoggerConfiguration()
//   .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
//   .CreateLogger();

builder.Services.AddScoped<IKeyVault, KeyVault>();
var keyVaultService = builder.Services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IKeyVault>();

//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(configuration)
//    .CreateLogger();


var sinkOptions = new MSSqlServerSinkOptions()
{
    TableName = "Logs",
    AutoCreateSqlTable = false
};

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
                  .WriteTo.MSSqlServer(keyVaultService.GetDatabaseConnectionStringSecret(), sinkOptions, null, null, Serilog.Events.LogEventLevel.Error)
                  .CreateLogger();
builder.Host.UseSerilog();

//builder.Services.AddDbContext<AppDbContext>(options =>
//           options.UseSqlServer(
//               builder.Configuration.GetConnectionString("DefaultConnection")
//               ));

builder.Services.AddDbContext<AppDbContext>(options =>
           options.UseSqlServer(
               keyVaultService.GetDatabaseConnectionStringSecret()
               ));
//builder.Services.AddSingleton(u => new BlobServiceClient(
//    builder.Configuration.GetValue<string>("StorageConnections")
//    ));


builder.Services.AddSingleton(u => new BlobServiceClient(
    keyVaultService.GetStorageConnectionStringSecret()
    ));

// Add services to the container.

#pragma warning disable CS0618 // Type or member is obsolete
builder.Services.AddControllers()
    .AddFluentValidation(c =>
    c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));
#pragma warning restore CS0618 // Type or member is obsolete

builder.Services.AddScoped<IContainerService, ContainerService>();
builder.Services.AddScoped<IBatchService, BatchService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Swagger API",
            Version = "v1"
        }
     );


    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(System.AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
}
);

builder.Services.AddTransient<CustomExceptionMiddleware>();

var app = builder.Build();



// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    if (!app.Environment.IsDevelopment())
    {
        c.RoutePrefix = string.Empty;
    }
}
);


app.UseHttpsRedirection();
app.UseAuthorization();

//Exception Handling

app.ConfigureCustomExceptionHandler();
app.MapControllers();

app.Run();
