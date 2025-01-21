namespace LocalUtilities.General;

public sealed class RandomGenerator
{
    static Random Random { get; set; } = new();

    public static void Reset()
    {
        Random = new();
    }

    public static (double X, double Y) GeneratePoint(
        double minX,
        double minY,
        double maxX,
        double maxY
    )
    {
        return GeneratePoints(minX, minY, maxX, maxY, 1).First();
    }

    public static List<(double X, double Y)> GeneratePoints(
        double minX,
        double minY,
        double maxX,
        double maxY,
        int count
    )
    {
        var sites = new List<(double X, double Y)>(count);
        for (int i = 0; i < count; i++)
        {
            sites.Add(new(nextGaussianRandom(minX, maxX), nextGaussianRandom(minY, maxY)));
        }
        return sites;
        double nextGaussianRandom(double min, double max)
        {
            // Box-Muller transform
            // From: https://stackoverflow.com/a/218600
            const double stdDev = 1.0 / 3.0; // this covers 99.73% of cases in (-1..1) range
            var mid = (max + min) / 2;
            do
            {
                var u1 = 1.0 - Random.NextDouble(); //uniform(0,1] random doubles
                var u2 = 1.0 - Random.NextDouble();
                var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                double value = stdDev * randStdNormal;
                double coord = mid + value * mid;
                if (coord > min && coord < max)
                    return coord;
            } while (true);
        }
    }

    public static T GenerateType<T>(bool ignoreFirstNoneElement)
        where T : Enum
    {
        var type = typeof(T);
        var counts = type.GetEnumValues().Length;
        var start = ignoreFirstNoneElement ? 1 : 0;
        return (T)Enum.ToObject(type, Random.Next(start, counts));
    }
}
