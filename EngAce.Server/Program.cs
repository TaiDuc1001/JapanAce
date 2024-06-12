using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EngAce APIs Documentation",
        Version = "v1.0.0",
        Description = "Developed by Phan Xuan Quang."
    });

    c.UseAllOfToExtendReferenceSchemas();
    c.MapType<ProblemDetails>(() => new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "ProblemDetails" } });
    c.UseAllOfToExtendReferenceSchemas();
    c.MapType<ValidationProblemDetails>(() => new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "ValidationProblemDetails" } });
    c.UseAllOfToExtendReferenceSchemas();
    c.MapType<SerializableError>(() => new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "SerializableError" } });

    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend APIs");

});

app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();