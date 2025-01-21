using AltitudeMapGenerator.Extensions;
using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities.General;

namespace AltitudeMapGenerator.VoronoiDiagram.BorderDisposal;

internal sealed class BorderClipping
{
    public static List<VoronoiEdge> Clip(
        List<VoronoiEdge> edges,
        double minX,
        double minY,
        double maxX,
        double maxY
    )
    {
        for (int i = 0; i < edges.Count; i++)
        {
            VoronoiEdge edge = edges[i];
            bool valid = ClipEdge(edge, minX, minY, maxX, maxY);
            if (valid)
            {
                edge.Left!.Edges.Add(edge);
                edge.Right!.Edges.Add(edge);
            }
            else
            {
                // Since the edge is not valid, then it also cannot "connect" sites as neighbours.
                // Technically, the sites are neighbours on an infinite place, but clipping at borders means foregoing such neighbouring.
                edge.Left!.Neighbours.Remove(edge.Right!);
                edge.Right!.Neighbours.Remove(edge.Left!);

                edges.RemoveAt(i);
                i--;
            }
        }
        return edges;
    }

    /// <summary>
    /// Combination of personal ray clipping alg and cohen sutherland
    /// </summary>
    private static bool ClipEdge(
        VoronoiEdge edge,
        double minX,
        double minY,
        double maxX,
        double maxY
    )
    {
        bool accept = false;
        // If it's a ray
        if (edge.Ender == null)
            accept = ClipRay(edge, minX, minY, maxX, maxY);
        else
        {
            //Cohen–Sutherland
            var start = ComputeOutCode(edge.Starter.X, edge.Starter.Y, minX, minY, maxX, maxY);
            var end = ComputeOutCode(edge.Ender.X, edge.Ender.Y, minX, minY, maxX, maxY);
            while (true)
            {
                if ((start | end) == Outcode.None)
                {
                    accept = true;
                    break;
                }
                if ((start & end) != Outcode.None)
                    break;
                double x = -1,
                    y = -1;
                var outcode = start != Outcode.None ? start : end;
                if (outcode.HasFlag(Outcode.Top))
                {
                    x =
                        edge.Starter.X
                        + (edge.Ender.X - edge.Starter.X)
                            * (maxY - edge.Starter.Y)
                            / (edge.Ender.Y - edge.Starter.Y);
                    y = maxY;
                }
                else if (outcode.HasFlag(Outcode.Bottom))
                {
                    x =
                        edge.Starter.X
                        + (edge.Ender.X - edge.Starter.X)
                            * (minY - edge.Starter.Y)
                            / (edge.Ender.Y - edge.Starter.Y);
                    y = minY;
                }
                else if (outcode.HasFlag(Outcode.Right))
                {
                    y =
                        edge.Starter.Y
                        + (edge.Ender.Y - edge.Starter.Y)
                            * (maxX - edge.Starter.X)
                            / (edge.Ender.X - edge.Starter.X);
                    x = maxX;
                }
                else if (outcode.HasFlag(Outcode.Left))
                {
                    y =
                        edge.Starter.Y
                        + (edge.Ender.Y - edge.Starter.Y)
                            * (minX - edge.Starter.X)
                            / (edge.Ender.X - edge.Starter.X);
                    x = minX;
                }
                var finalPoint = new VoronoiVertex(
                    x,
                    y,
                    GetBorderLocationForCoordinate(x, y, minX, minY, maxX, maxY)
                );
                if (outcode == start)
                {
                    // If we are a 0-length edge after clipping, then we are a "connector" between more than 2 equidistant sites
                    if (finalPoint == edge.Ender)
                    {
                        // We didn't consider this point to be on border before, so need reflag it
                        edge.Ender.DirectionOnBorder = finalPoint.DirectionOnBorder;
                        // (point is shared between edges, so we are basically setting this for all the other edges)

                        // The neighbours in-between (ray away outside the border) are not actually connected
                        edge.Left!.Neighbours.Remove(edge.Right!);
                        edge.Right!.Neighbours.Remove(edge.Left!);

                        // Not a valid edge
                        return false;
                    }
                    edge.Starter = finalPoint;
                    start = ComputeOutCode(x, y, minX, minY, maxX, maxY);
                }
                else
                {
                    // If we are a 0-length edge after clipping, then we are a "connector" between more than 2 equidistant sites
                    if (finalPoint == edge.Starter)
                    {
                        // We didn't consider this point to be on border before, so need reflag it
                        edge.Starter.DirectionOnBorder = finalPoint.DirectionOnBorder;
                        // (point is shared between edges, so we are basically setting this for all the other edges)

                        // The neighbours in-between (ray away outside the border) are not actually connected
                        edge.Left!.Neighbours.Remove(edge.Right!);
                        edge.Right!.Neighbours.Remove(edge.Left!);

                        // Not a valid edge
                        return false;
                    }
                    edge.Ender = finalPoint;
                    end = ComputeOutCode(x, y, minX, minY, maxX, maxY);
                }
            }
        }
        // If we have a neighbor
        if (edge.LastBeachLineNeighbor != null)
        {
            // Check it
            bool valid = ClipEdge(edge.LastBeachLineNeighbor, minX, minY, maxX, maxY);
            // Both are valid
            if (accept && valid)
                edge.Starter = edge.LastBeachLineNeighbor.Ender;
            // This edge isn't valid, but the neighbor is
            // Flip and set
            if (!accept && valid)
            {
                edge.Starter = edge.LastBeachLineNeighbor.Ender;
                edge.Ender = edge.LastBeachLineNeighbor.Starter;
                accept = true;
            }
        }
        return accept;
    }

    private static Outcode ComputeOutCode(
        double x,
        double y,
        double minX,
        double minY,
        double maxX,
        double maxY
    )
    {
        var code = Outcode.None;
        if (x.ApproxEqualTo(minX) || x.ApproxEqualTo(maxX)) { }
        else if (x < minX)
            code |= Outcode.Left;
        else if (x > maxX)
            code |= Outcode.Right;
        if (y.ApproxEqualTo(minY) || y.ApproxEqualTo(maxY)) { }
        else if (y < minY)
            code |= Outcode.Bottom;
        else if (y > maxY)
            code |= Outcode.Top;
        return code;
    }

    private static bool ClipRay(
        VoronoiEdge edge,
        double minX,
        double minY,
        double maxX,
        double maxY
    )
    {
        VoronoiVertex start = edge.Starter;

        //horizontal ray
        if (edge.SlopeRise.ApproxEqualTo(0))
        {
            if (!Within(start.Y, minY, maxY))
                return false;
            if (edge.SlopeRun.ApproxGreaterThan(0) && start.X.ApproxGreaterThan(maxX))
                return false;
            if (edge.SlopeRun.ApproxLessThan(0) && start.X.ApproxLessThan(minX))
                return false;
            if (Within(start.X, minX, maxX))
            {
                VoronoiVertex endPoint = edge.SlopeRun.ApproxGreaterThan(0)
                    ? new VoronoiVertex(maxX, start.Y, Direction.Right)
                    : new VoronoiVertex(
                        minX,
                        start.Y,
                        start.Y.ApproxEqualTo(minY) ? Direction.LeftTop
                            : start.Y.ApproxEqualTo(maxY) ? Direction.LeftBottom
                            : Direction.Left
                    );
                // If we are a 0-length edge after clipping, then we are a "connector" between more than 2 equidistant sites
                if (endPoint == edge.Starter)
                {
                    // We didn't consider this point to be on border before, so need reflag it
                    start.DirectionOnBorder = endPoint.DirectionOnBorder;
                    // (point is shared between edges, so we are basically setting this for all the other edges)

                    // The neighbours in-between (ray away outside the border) are not actually connected
                    edge.Left!.Neighbours.Remove(edge.Right!);
                    edge.Right!.Neighbours.Remove(edge.Left!);
                    // Not a valid edge
                    return false;
                }
                edge.Ender = endPoint;
            }
            else
            {
                if (edge.SlopeRun.ApproxGreaterThan(0))
                {
                    edge.Starter = new VoronoiVertex(minX, start.Y, Direction.Left);
                    edge.Ender = new VoronoiVertex(maxX, start.Y, Direction.Right);
                }
                else
                {
                    edge.Starter = new VoronoiVertex(maxX, start.Y, Direction.Right);
                    edge.Ender = new VoronoiVertex(minX, start.Y, Direction.Left);
                }
            }
            return true;
        }
        //vertical ray
        if (edge.SlopeRun.ApproxEqualTo(0))
        {
            if (start.X.ApproxLessThan(minX) || start.X.ApproxGreaterThan(maxX))
                return false;
            if (edge.SlopeRise.ApproxGreaterThan(0) && start.Y.ApproxGreaterThan(maxY))
                return false;
            if (edge.SlopeRise.ApproxLessThan(0) && start.Y.ApproxLessThan(minY))
                return false;
            if (Within(start.Y, minY, maxY))
            {
                VoronoiVertex endPoint = edge.SlopeRise.ApproxGreaterThan(0)
                    ? new VoronoiVertex(
                        start.X,
                        maxY,
                        start.X.ApproxEqualTo(minX) ? Direction.LeftBottom
                            : start.X.ApproxEqualTo(maxX) ? Direction.BottomRight
                            : Direction.Bottom
                    )
                    : new VoronoiVertex(start.X, minY, Direction.Top);
                // If we are a 0-length edge after clipping, then we are a "connector" between more than 2 equidistant sites
                if (endPoint == edge.Starter)
                {
                    // We didn't consider this point to be on border before, so need reflag it
                    start.DirectionOnBorder = endPoint.DirectionOnBorder;
                    // (point is shared between edges, so we are basically setting this for all the other edges)

                    // The neighbours in-between (ray away outside the border) are not actually connected
                    edge.Left!.Neighbours.Remove(edge.Right!);
                    edge.Right!.Neighbours.Remove(edge.Left!);

                    // Not a valid edge
                    return false;
                }
                edge.Ender = endPoint;
            }
            else
            {
                if (edge.SlopeRise.ApproxGreaterThan(0))
                {
                    edge.Starter = new VoronoiVertex(start.X, minY, Direction.Top);
                    edge.Ender = new VoronoiVertex(start.X, maxY, Direction.Bottom);
                }
                else
                {
                    edge.Starter = new VoronoiVertex(start.X, maxY, Direction.Bottom);
                    edge.Ender = new VoronoiVertex(start.X, minY, Direction.Top);
                }
            }
            return true;
        }
        //works for outside
        double topXValue = CalcX(edge.Slope!.Value, maxY, edge.Intercept!.Value);
        var topX = new VoronoiVertex(
            topXValue,
            maxY,
            topXValue.ApproxEqualTo(minX) ? Direction.LeftBottom
                : topXValue.ApproxEqualTo(maxX) ? Direction.BottomRight
                : Direction.Bottom
        );
        double rightYValue = CalcY(edge.Slope.Value, maxX, edge.Intercept.Value);
        var rightY = new VoronoiVertex(
            maxX,
            rightYValue,
            rightYValue.ApproxEqualTo(minY) ? Direction.TopRight
                : rightYValue.ApproxEqualTo(maxY) ? Direction.BottomRight
                : Direction.Right
        );
        double bottomXValue = CalcX(edge.Slope.Value, minY, edge.Intercept.Value);
        var bottomX = new VoronoiVertex(
            bottomXValue,
            minY,
            bottomXValue.ApproxEqualTo(minX) ? Direction.LeftTop
                : bottomXValue.ApproxEqualTo(maxX) ? Direction.TopRight
                : Direction.Top
        );
        double leftYValue = CalcY(edge.Slope.Value, minX, edge.Intercept.Value);
        var leftY = new VoronoiVertex(
            minX,
            leftYValue,
            leftYValue.ApproxEqualTo(minY) ? Direction.LeftTop
                : leftYValue.ApproxEqualTo(maxY) ? Direction.LeftBottom
                : Direction.Left
        );

        // Note: these points may be duplicates if the ray goes through a border corner,
        // so we have to check for repeats when building the candidate list below.
        // We can optimize slightly since we are adding them one at a time and only "neighbouring" points can be the same,
        // e.g. topX and rightY can but not topX and bottomX.

        //reject intersections not within bounds

        List<VoronoiVertex> candidates = new List<VoronoiVertex>();

        bool withinTopX = Within(topX.X, minX, maxX);
        bool withinRightY = Within(rightY.Y, minY, maxY);
        bool withinBottomX = Within(bottomX.X, minX, maxX);
        bool withinLeftY = Within(leftY.Y, minY, maxY);

        if (withinTopX)
            candidates.Add(topX);

        if (withinRightY)
            if (!withinTopX || !(rightY == topX))
                candidates.Add(rightY);

        if (withinBottomX)
            if (!withinRightY || !(bottomX == rightY))
                candidates.Add(bottomX);

        if (withinLeftY)
            if (!withinTopX || !(leftY == topX))
                if (!withinBottomX || !(leftY == bottomX))
                    candidates.Add(leftY);

        // This also works as a condition above, but is slower and checks against redundant values
        // if (candidates.All(c => !c.X.ApproxEqual(leftY.X) || !c.Y.ApproxEqual(leftY.Y)))


        //reject candidates which don't align with the slope
        for (int i = candidates.Count - 1; i >= 0; i--)
        {
            VoronoiVertex candidate = candidates[i];
            //grab vector representing the edge
            double ax = candidate.X - start.X;
            double ay = candidate.Y - start.Y;
            if ((edge.SlopeRun * ax + edge.SlopeRise * ay).ApproxLessThan(0))
                candidates.RemoveAt(i);
        }

        // If there are two candidates, we are outside the plane.
        // The closer candidate is the start point while the further one is the end point.
        if (candidates.Count == 2)
        {
            double ax = candidates[0].X - start.X;
            double ay = candidates[0].Y - start.Y;
            double bx = candidates[1].X - start.X;
            double by = candidates[1].Y - start.Y;

            if ((ax * ax + ay * ay).ApproxGreaterThan(bx * bx + by * by))
            {
                // Candidate 1 is closer

                if (!(edge.Starter == candidates[1]))
                    edge.Starter = candidates[1];
                // If the point is already at the right location (i.e. edge.Start == candidates[1]), then keep it.
                // This preserves the same instance between potential multiple edges.
                // If not, it's a new clipped point, which will be unique

                // It didn't have a border location being an unfinished edge point.
                edge.Starter.DirectionOnBorder = GetBorderLocationForCoordinate(
                    edge.Starter.X,
                    edge.Starter.Y,
                    minX,
                    minY,
                    maxX,
                    maxY
                );

                // The other point, i.e. end, didn't have a value yet
                edge.Ender = candidates[0]; // candidate point already has the border location set correctly
            }
            else
            {
                // Candidate 2 is closer

                if (!(edge.Starter == candidates[0]))
                    edge.Starter = candidates[0];
                // If the point is already at the right location (i.e. edge.Start == candidates[0]), then keep it.
                // This preserves the same instance between potential multiple edges.
                // If not, it's a new clipped point, which will be unique

                // It didn't have a border location being an unfinished edge point.
                edge.Starter.DirectionOnBorder = GetBorderLocationForCoordinate(
                    edge.Starter.X,
                    edge.Starter.Y,
                    minX,
                    minY,
                    maxX,
                    maxY
                );

                // The other point, i.e. end, didn't have a value yet
                edge.Ender = candidates[1]; // candidate point already has the border location set correctly
            }
        }

        // If there is one candidate, we are inside the plane
        if (candidates.Count == 1)
        {
            // If we are already at the candidate point, then we are already on the border at the "clipping" location
            if (candidates[0] == edge.Starter)
            {
                // We didn't consider this point to be on border before, so need reflag it
                start.DirectionOnBorder = candidates[0].DirectionOnBorder;
                // (point is shared between edges, so we are basically setting this for all the other edges)

                // We did not actually clip anything, we are already clipped correctly, so to speak
                return false;
            }

            // Start remains as is

            // The other point has a value now
            edge.Ender = candidates[0]; // candidate point already has the border location set correctly
        }

        // There were no candidates
        return edge.Ender != null!; // can be null for now until we fully clip it
    }

    private static bool Within(double x, double a, double b)
    {
        return x.ApproxGreaterThanOrEqualTo(a) && x.ApproxLessThanOrEqualTo(b);
    }

    private static double CalcY(double m, double x, double b)
    {
        return m * x + b;
    }

    private static double CalcX(double m, double y, double b)
    {
        return (y - b) / m;
    }

    private static Direction GetBorderLocationForCoordinate(
        double x,
        double y,
        double minX,
        double minY,
        double maxX,
        double maxY
    )
    {
        if (x.ApproxEqualTo(minX) && y.ApproxEqualTo(minY))
            return Direction.LeftTop;
        if (x.ApproxEqualTo(minX) && y.ApproxEqualTo(maxY))
            return Direction.LeftBottom;
        if (x.ApproxEqualTo(maxX) && y.ApproxEqualTo(minY))
            return Direction.TopRight;
        if (x.ApproxEqualTo(maxX) && y.ApproxEqualTo(maxY))
            return Direction.BottomRight;

        if (x.ApproxEqualTo(minX))
            return Direction.Left;
        if (y.ApproxEqualTo(minY))
            return Direction.Top;
        if (x.ApproxEqualTo(maxX))
            return Direction.Right;
        if (y.ApproxEqualTo(maxY))
            return Direction.Bottom;

        return Direction.None;
    }

    [Flags]
    private enum Outcode
    {
        None = 0x0,
        Left = 0x1,
        Right = 0x2,
        Bottom = 0x4,
        Top = 0x8,
    }
}
