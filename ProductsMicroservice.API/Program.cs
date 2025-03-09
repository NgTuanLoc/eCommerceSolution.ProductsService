using DataAccessLayer;
using BusinessLogicLayer;
using FluentValidation.AspNetCore;
using ProductsMicroservice.API.Middleware;
using ProductsMicroservice.API.Endpoints;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();

// Add model binder to read values from JSON request to enum
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add Swagger Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

// Cors
app.UseCors();

// Swagger
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapProductEndpoints();

await app.RunAsync();
