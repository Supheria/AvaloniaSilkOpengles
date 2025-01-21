using AltitudeMapGenerator.DLA;
using AltitudeMapGenerator.VoronoiDiagram;
using Avalonia;
using LocalUtilities.General;

namespace AltitudeMapGenerator;

public sealed class AltitudeMap
{
    public PixelRect Bounds { get; private set; }

    public double AltitudeMax { get; private set; }

    public List<PixelPoint> OriginPoints { get; private set; } = [];

    public List<PixelPoint> RiverPoints { get; private set; } = [];

    public List<AltitudePoint> AltitudePoints { get; private set; } = [];

    public int Width => Bounds.Width;

    public int Height => Bounds.Height;

    public PixelSize Size => Bounds.Size;

    public int Area => Bounds.Width * Bounds.Height;

    public AltitudeMap() { }

    public AltitudeMap(AltitudeMapData data, IProgressor? progressor)
    {
        VoronoiPlane plane;
        List<PixelPoint> sites;
        RiverGenerator river;
        do
        {
            Bounds = new(data.Size);
            plane = new VoronoiPlane(data.Size);
            sites = plane.GenerateSites(data.SegmentNumber);
            river = new RiverGenerator(
                data.RiverWidth,
                data.Size,
                data.RiverSegmentNumber,
                data.RiverLayoutType,
                sites
            );
        } while (river.Successful is false);
        RiverPoints = river.River.ToList();

        progressor?.Reset(data.PixelNumber);
        DlaMap.Progressor = progressor;

        var pixels = new List<DlaPixel>();
        var altitudes = new List<double>();
        var origins = new List<PixelPoint>();
        Parallel.ForEach(
            plane.Generate(sites),
            (cell) =>
            {
                var dlaMap = new DlaMap(cell);
                pixels.AddRange(
                    dlaMap.Generate((int)(cell.Area / Area * data.PixelNumber), data.PixelDensity)
                );
                altitudes.Add(dlaMap.AltitudeMax);
                origins.Add(cell.Site);
            }
        );
        OriginPoints = origins;
        var altitudesPoints = new Roster<PixelPoint, AltitudePoint>();
        foreach (var pixel in pixels)
        {
            var coordinate = new PixelPoint(pixel.X, pixel.Y);
            if (altitudesPoints.TryGetValue(coordinate, out AltitudePoint? point))
            {
                altitudesPoints[coordinate] = new()
                {
                    Site = coordinate,
                    Altitude = point.Altitude + pixel.Altitude,
                };
                altitudes.Add(altitudesPoints[coordinate].Altitude);
            }
            else
                altitudesPoints[coordinate] = new()
                {
                    Site = coordinate,
                    Altitude = pixel.Altitude,
                };
        }
        AltitudePoints = altitudesPoints.ToList();
        AltitudeMax = altitudes.Max();
    }
}
