using TestProjectMap.Services;
using Microsoft.AspNetCore.Mvc;
using TestProjectMap.Endpoints.Requests;

namespace TestProjectMap.Endpoints;

public sealed class FieldEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var fieldGroup = app.MapGroup("/fields")
            .WithTags("fields");

        fieldGroup.MapGet("/", GetFields);
        fieldGroup.MapGet("/{id:int}/size", GetFieldSize);
        fieldGroup.MapPost("/{id:int}/distance", GetDistanceToFieldCenter);
        fieldGroup.MapPost("/contains", ContainsPoint);
    }
    private static IResult GetFields(IFieldService service)
    {
        var fields = service.GetFields();
        return Results.Ok(fields);
    }

    private static IResult GetFieldSize(int id, IFieldService service)
    {
        var size = service.GetFieldSize(id);
        return Results.Ok(size);
    }

    private static IResult GetDistanceToFieldCenter(int id, [FromBody] DistanceRequest req, IFieldService service)
    {
        var distance = service.GetDistance(id, req.Lat, req.Lon);
        return Results.Ok(distance);
    }

    private static IResult ContainsPoint([FromBody] ContainsRequest req, IFieldService service)
    {
        var result = service.ContainsPoint(req.Lat, req.Lon);
        if (result == null)
            return Results.NotFound("Not found");
        return Results.Ok(result);
    }
}
