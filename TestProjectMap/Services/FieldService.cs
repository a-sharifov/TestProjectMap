using System.Xml.Linq;
using TestProjectMap.Models;
using System.Globalization;

namespace TestProjectMap.Services;

internal class FieldService : IFieldService
{
    private static readonly XNamespace Namespace = "http://www.opengis.net/kml/2.2";
    private const string FieldsPath = "Data/fields.kml";
    private const string CentroidsPath = "Data/centroids.kml";

    public IEnumerable<Field> GetFields()
    {
        var xFieldsDocs = XDocument.Load(FieldsPath);
        var xCentroidsDocs = XDocument.Load(CentroidsPath);

        var centroidDict = GetPlacemarks(xCentroidsDocs)
            .Select(p => new { Placemark = p, Fid = GetSimpleDataInt(p, "fid") })
            .Where(x => x.Fid.HasValue)
            .ToDictionary(x => x.Fid!.Value, x => x.Placemark);

        var fields = GetPlacemarks(xFieldsDocs);
        var fieldDtos = new List<Field>(fields.Count);

        foreach (var fieldPlacemark in fields)
        {
            var id = GetSimpleDataInt(fieldPlacemark, "fid");
            if (!id.HasValue) continue;
            var size = GetSimpleDataDouble(fieldPlacemark, "size") ?? 0d;
            var name = fieldPlacemark.Descendants(Namespace + "name").FirstOrDefault()?.Value ?? string.Empty;

            var center = new Point(0, 0);
            if (centroidDict.TryGetValue(id.Value, out var centroidPlacemark))
            {
                var c = ParseCoordinates(centroidPlacemark.Descendants(Namespace + "coordinates").FirstOrDefault()?.Value).FirstOrDefault();
                if (c != null) center = c;
            }

            var polygon = ParseCoordinates(fieldPlacemark.Descendants(Namespace + "coordinates").FirstOrDefault()?.Value);

            fieldDtos.Add(new Field
            (
                id.Value,
                name,
                size,
                new FieldLocation(center, polygon)
            ));
        }

        return fieldDtos;
    }

    public double GetFieldSize(int id)
    {
        var xFieldsDocs = XDocument.Load(FieldsPath);
        var placemark = xFieldsDocs.Descendants(Namespace + "Placemark")
            .FirstOrDefault(p => GetSimpleDataInt(p, "fid") == id);
        return placemark != null ? GetSimpleDataDouble(placemark, "size") ?? -1 : -1;
    }

    public double GetDistance(int fieldId, double latitude, double longitude)
    {
        var fields = GetFields();
        var field = fields.FirstOrDefault(f => f.Id == fieldId);
        if (field == null)
            return -1;
        var center = field.Locations.Center;
        return Haversine(center.Lat, center.Lon, latitude, longitude);
    }

    public object ContainsPoint(double latitude, double longitude)
    {
        var fields = GetFields();
        var point = new Point(longitude, latitude);
        foreach (var field in fields)
        {
            if (PointInPolygon(point, field.Locations.Polygon))
                return new { id = field.Id, name = field.Name };
        }
        return false;
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000;
        var latRad1 = lat1 * Math.PI / 180.0;
        var latRad2 = lat2 * Math.PI / 180.0;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(latRad1) * Math.Cos(latRad2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static bool PointInPolygon(Point point, List<Point> polygon)
    {
        int n = polygon.Count;
        bool inside = false;
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            var xi = polygon[i].Lon;
            var yi = polygon[i].Lat;
            var xj = polygon[j].Lon;
            var yj = polygon[j].Lat;
            if (((yi > point.Lat) != (yj > point.Lat)) &&
                (point.Lon < (xj - xi) * (point.Lat - yi) / (yj - yi + double.Epsilon) + xi))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    private static List<XElement> GetPlacemarks(XDocument doc) =>
        doc.Element(Namespace + "kml")?
           .Element(Namespace + "Document")?
           .Element(Namespace + "Folder")?
           .Elements(Namespace + "Placemark")
           .ToList() ?? [];

    private static int? GetSimpleDataInt(XElement placemark, string name)
    {
        var value = placemark.Descendants(Namespace + "SimpleData")
            .FirstOrDefault(sd => sd.Attribute("name")?.Value == name)?.Value;
        return int.TryParse(value, out var result) ? result : (int?)null;
    }

    private static double? GetSimpleDataDouble(XElement placemark, string name)
    {
        var value = placemark.Descendants(Namespace + "SimpleData")
            .FirstOrDefault(sd => sd.Attribute("name")?.Value == name)?.Value;
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : (double?)null;
    }

    private static List<Point> ParseCoordinates(string? coordinates)
    {
        var result = new List<Point>();
        if (string.IsNullOrWhiteSpace(coordinates))
            return result;
        var pairs = coordinates.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in pairs)
        {
            var parts = pair.Split(',');
            if (parts.Length >= 2 &&
                double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
            {
                result.Add(new Point(x, y));
            }
        }
        return result;
    }
}