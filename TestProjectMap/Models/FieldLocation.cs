namespace TestProjectMap.Models;

public class FieldLocation
{
    public (float, float) Center { get; set; }        
    public List<(float, float)> Polygon { get; set; }
}
