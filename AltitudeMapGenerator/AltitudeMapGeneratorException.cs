using LocalUtilities.General;

namespace AltitudeMapGenerator;

public sealed class AltitudeMapGeneratorException(string message) : Exception(message)
{
    public static AltitudeMapGeneratorException NotProperRiverLayoutType()
    {
        return new($"not a proper type of river layout");
    }

    public static AltitudeMapGeneratorException NotProperRiverEndnodeDirection(Direction direction)
    {
        return new($"{direction} is not proper to river end node");
    }

    public static AltitudeMapGeneratorException AltitudeRatioOutRange()
    {
        return new($"altitude ratio is out of range, it should between 0 and 1");
    }
}
