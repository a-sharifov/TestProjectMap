using TestProjectMap.Services;

namespace TestProjectMap.Endpoints;

public sealed class GeoEndpoints : CarterModule
{
    private static IResult GetFields(FieldService service)
    {
        var fields = service.GetFields();
        return Results.Ok(fields);
    }

    private static IResult GetFieldSize(int id, FieldService service)
    {
        var size = service.GetFieldSize(id);
        return Results.Ok(size);
    }

    private static IResult GetDistance()
    {
        return Results.Ok("Get distance between two points");
    }

    private static IResult GetContains()
    {
        return Results.Ok("Check if a point is contained within a field");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        for (int i = 0; i < 1000; i++)
        {
            Console.WriteLine(i);
        }

        var geoGroup = app.MapGroup("/fields")
            .WithTags("Geo");

        geoGroup.MapGet("/", GetFields);
        geoGroup.MapGet("/{id:int}/size", GetFieldSize);
        geoGroup.MapGet("/distance", GetDistance);
        geoGroup.MapGet("/contains", GetContains);
    }
}
