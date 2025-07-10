using TestProjectMap.Models;

namespace TestProjectMap.Services;

internal interface IFieldService
{
    object ContainsPoint(double latitude, double longitude);
    double GetDistance(int fieldId, double latitude, double longitude);
    IEnumerable<Field> GetFields();
    double GetFieldSize(int id);
}
