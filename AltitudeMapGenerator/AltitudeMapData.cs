using AltitudeMapGenerator.Layout;
using Avalonia;

namespace AltitudeMapGenerator;

public sealed class AltitudeMapData(
    PixelSize size,
    PixelSize segmentNumber,
    PixelSize riverSegmentNumber,
    RiverLayoutType riverLayoutType,
    double riverWidth,
    int pixelNumber,
    double pixelDensity
)
{
    public PixelSize Size { get; private set; } = size;
    public PixelSize SegmentNumber { get; private set; } = segmentNumber;
    public PixelSize RiverSegmentNumber { get; private set; } = riverSegmentNumber;
    public double RiverWidth { get; private set; } = riverWidth;
    public RiverLayoutType RiverLayoutType { get; private set; } = riverLayoutType;
    public int PixelNumber { get; private set; } = pixelNumber;
    public double PixelDensity { get; private set; } = pixelDensity;
}
