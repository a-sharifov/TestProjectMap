namespace TestProjectMap.Models;

public class FieldDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Size { get; set; }
    public FieldLocation Locations { get; set; }
}
