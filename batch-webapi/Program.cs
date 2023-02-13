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

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

//Log.Logger = new LoggerConfiguration()
//   .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
//   .CreateLogger();

builder.Services.AddDbContext<AppDbContext>(options =>
           options.UseSqlServer(
               builder.Configuration.GetConnectionString("DefaultConnection")
               ));


// Add services to the container.

#pragma warning disable CS0618 // Type or member is obsolete
builder.Services.AddControllers()
    .AddFluentValidation(c =>
    c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));
#pragma warning restore CS0618 // Type or member is obsolete


builder.Services.AddTransient<BatchService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<CustomExceptionMiddleware>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

//Exception Handling

app.ConfigureCustomExceptionHandler();
app.MapControllers();

app.Run();
