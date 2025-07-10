namespace TestProjectMap.Models;

internal sealed record Field(
    int Id, 
    string Name, 
    double Size, 
    FieldLocation Locations);
