namespace AltitudeMapGenerator.VoronoiDiagram.Model;

interface IFortuneEvent : IComparable<IFortuneEvent>
{
    double X { get; }
    double Y { get; }
}
