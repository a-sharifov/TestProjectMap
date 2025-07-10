using System.Drawing;
using System.Reflection.Metadata;
using System.Text;
using System;
using System.Xml.Linq;
using TestProjectMap.Models;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Globalization;

namespace TestProjectMap.Services;

internal class FieldService
{
    private static readonly XNamespace Namespace = "http://www.opengis.net/kml/2.2";

    public IEnumerable<FieldDto> GetFields()
    {
        var xFieldsDocs = XDocument.Load("Data/fields.kml");
        var xCentroidsDocs = XDocument.Load("Data/centroids.kml");

        var fields = xFieldsDocs.Descendants(Namespace + "Placemark").ToList();
        var centroids = xCentroidsDocs.Descendants(Namespace + "Placemark").ToList();

        var fieldDtos = new List<FieldDto>();

        for (int i = 0; i < fields.Count; i++)
        {
            var field = new FieldDto
            {
                Id = int.Parse(fields[i].Descendants(Namespace + "SimpleData")
                .First(sd => sd.Attribute("name")?.Value == "fid").Value),
                Size = float.Parse(fields[i].Descendants(Namespace + "SimpleData")
                    .First(sd => sd.Attribute("name")?.Value == "size").Value),
                Name = fields[i].Descendants(Namespace + "name").First().Value,
                Locations = new FieldLocation
                {
                    Center = new(
                        float.Parse(centroids[i].Descendants(Namespace + "coordinates")
                        .First().Value.Split(',')[0], CultureInfo.InvariantCulture),
                         float.Parse(centroids[i].Descendants(Namespace + "coordinates")
                        .First().Value.Split(',')[1], CultureInfo.InvariantCulture)),

                    Polygon = fields[i].Descendants(Namespace + "coordinates").ToList()
                        .Select(c => c.Value.Split(' ')
                        .Select(coord => coord.Split(','))
                        .Select(coordParts => (
                            float.Parse(coordParts[0], CultureInfo.InvariantCulture), float.Parse(coordParts[1], CultureInfo.InvariantCulture)
                            ))
                        .ToList())
                        .First()
                }
            };

            fieldDtos.Add(field);
        }

        return fieldDtos;
    }
    public float GetFieldSize(int id)
    {
        var xFieldsDocs = XDocument.Load("Data/fields.kml");

        XNamespace ns = "http://www.opengis.net/kml/2.2";

        var placemark = xFieldsDocs.Descendants(ns + "Placemark")
            .FirstOrDefault(p => p.Descendants(ns + "SimpleData")
                .Any(sd => sd.Attribute("name")?.Value == "fid" &&
                          int.TryParse(sd.Value, out int currentFid) &&
                          currentFid == id));

        if (placemark != null)
        {
            var sizeElement = placemark.Descendants(ns + "SimpleData")
                .FirstOrDefault(sd => sd.Attribute("name")?.Value == "size");

            if (sizeElement != null
                && float.TryParse(sizeElement.Value, out float size))
            {
                return size;
            }
        }

        return -1;
    }
}
//public string GetDistance()
//{
//    return "Get distance between two points";
//}
//public string GetContains()
//{
//    return "Check if a point is contained within a field";
//}
