using Avalonia;
using LocalUtilities.General;

namespace AltitudeMapGenerator.Layout;

internal static class RiverLayoutTypeExtension
{
    public static Func<PixelSize, RiverLayout> Parse(this RiverLayoutType type)
    {
        // [Horizontal]   [Vertical)  [ForwardSlash)  [BackwardSlash)
        //    _______      _______       _______          _______
        //   | _____ |    | |   | |     |    /  |        |  \    |
        //   |       |    | |   | |     |      /|        |\      |
        //   |       |    | |   | |     |/      |        |      \|
        //   | ----- |    | |   | |     |  /    |        |    \  |
        //    -------      -------       -------          -------

        return type switch
        {
            RiverLayoutType.Horizontal => (size) =>
                new(
                    (
                        new(Direction.Left, OperatorType.LessThan, size),
                        new(Direction.Right, OperatorType.LessThan, size)
                    ),
                    (
                        new(Direction.Left, OperatorType.GreaterThan, size),
                        new(Direction.Right, OperatorType.GreaterThan, size)
                    )
                ),
            RiverLayoutType.Vertical => (size) =>
                new(
                    (
                        new(Direction.Top, OperatorType.LessThan, size),
                        new(Direction.Bottom, OperatorType.LessThan, size)
                    ),
                    (
                        new(Direction.Top, OperatorType.GreaterThan, size),
                        new(Direction.Bottom, OperatorType.GreaterThan, size)
                    )
                ),
            RiverLayoutType.ForwardSlash => (size) =>
                new(
                    (
                        new(Direction.Top, OperatorType.GreaterThanOrEqualTo, size),
                        new(Direction.Left, OperatorType.GreaterThanOrEqualTo, size)
                    ),
                    (
                        new(Direction.Right, OperatorType.LessThanOrEqualTo, size),
                        new(Direction.Bottom, OperatorType.LessThanOrEqualTo, size)
                    )
                ),
            RiverLayoutType.BackwardSlash => (size) =>
                new(
                    (
                        new(Direction.Left, OperatorType.LessThanOrEqualTo, size),
                        new(Direction.Bottom, OperatorType.GreaterThanOrEqualTo, size)
                    ),
                    (
                        new(Direction.Top, OperatorType.LessThanOrEqualTo, size),
                        new(Direction.Right, OperatorType.GreaterThanOrEqualTo, size)
                    )
                ),
            RiverLayoutType.OneForTest => (size) =>
                new(
                    (
                        new(Direction.Top, OperatorType.GreaterThanOrEqualTo, size),
                        new(Direction.Left, OperatorType.GreaterThanOrEqualTo, size)
                    )
                ),
            _ => throw new InvalidOperationException(),
        };
    }
}
