using System.Reflection;
using TestProjectMap;
using TestProjectMap.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<FieldService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCarter(
    new DependencyContextAssemblyCatalog(AssemblyReference.Assembly));

builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCarter();

app.Run();

