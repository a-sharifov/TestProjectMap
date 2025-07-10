using TestProjectMap;
using TestProjectMap.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddYamlFile(
    "appsettings.yaml", optional: true, reloadOnChange: true);

builder.Services.AddTransient<IFieldService, FieldService>();
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

