namespace TestProjectMap.Models;

internal record FieldLocation(
    Point Center, 
    List<Point> Polygon);
