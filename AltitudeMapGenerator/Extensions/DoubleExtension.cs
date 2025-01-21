using LocalUtilities.General;

namespace AltitudeMapGenerator.Extensions;

internal static class DoubleExtension
{
    //private const double epsilon = double.Epsilon * 1E100;
    // The above is the original epsilon used for the algorithm.
    // This doesn't actually work since precision is lost after 15 digits for double.
    // Even with multiplier it's still like 4.94E-224, which is ridiculously beyond the loss of precision threshold.
    // For example, beach line edge intercept stuff would only capture precision to something like:
    // 999.99999999999989 vs 1000
    // In fact, anything less than e^-12 will immediatelly fail some coordinate comparisons because of all the compounding precision losses.
    // Of course, numbers too large will start failing again since we can't exactly compare significant digits (cheaply).
    static double Epsilon { get; } = 1E-12;

    public static int ToRoundInt(this double value)
    {
        return (int)Math.Round(value);
    }

    public static bool ApproxEqualTo(this double value1, double value2)
    {
        return value1 - value2 < Epsilon && value2 - value1 < Epsilon;
    }

    public static bool ApproxGreaterThan(this double value1, double value2)
    {
        return value1 > value2 + Epsilon;
    }

    public static bool ApproxGreaterThanOrEqualTo(this double value1, double value2)
    {
        return value1 > value2 - Epsilon;
    }

    public static bool ApproxLessThan(this double value1, double value2)
    {
        return value1 < value2 - Epsilon;
    }

    public static bool ApproxLessThanOrEqualTo(this double value1, double value2)
    {
        return value1 < value2 + Epsilon;
    }

    public static int ApproxCompareTo(this double value1, double value2)
    {
        if (value1.ApproxGreaterThan(value2))
            return 1;

        if (value1.ApproxLessThan(value2))
            return -1;

        return 0;
    }

    public static bool ApproxOperatorTo(this double value1, OperatorType type, double value2)
    {
        return type switch
        {
            OperatorType.Equal => value1.ApproxEqualTo(value2),
            OperatorType.LessThan => value1.ApproxLessThan(value2),
            OperatorType.GreaterThan => value1.ApproxGreaterThan(value2),
            OperatorType.LessThanOrEqualTo => value1.ApproxLessThanOrEqualTo(value2),
            OperatorType.GreaterThanOrEqualTo => value1.ApproxGreaterThanOrEqualTo(value2),
            _ => throw new InvalidOperationException(),
        };
    }
}
