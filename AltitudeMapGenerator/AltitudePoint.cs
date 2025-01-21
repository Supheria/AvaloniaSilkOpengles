using Avalonia;
using LocalUtilities.General;

namespace AltitudeMapGenerator;

public sealed class AltitudePoint : IRosterItem<PixelPoint>
{
    public PixelPoint Site { get; set; }
    public PixelPoint Signature => Site;

    public double Altitude { get; set; }
}
