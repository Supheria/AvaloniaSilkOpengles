using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities.General;

namespace AltitudeMapGenerator.VoronoiDiagram.BorderDisposal;

internal sealed class BorderClosing
{
    public static List<VoronoiEdge> Close(
        List<VoronoiEdge> edges,
        double minX,
        double minY,
        double maxX,
        double maxY,
        List<VoronoiCell> cells
    )
    {
        // We construct edges in clockwise order on the border:
        // →→→→→→→→↓
        // ↑       ↓
        // ↑       ↓
        // ↑       ↓
        // O←←←←←←←←
        //
        // We construct edges between nodes on this border.
        // Nodes are points that need edges between them and are either:
        // * Edge start/end points (any edge touching the border "breaks" it into two sections except if it's in a corner)
        // * Corner points (unless an edge ends in a corner, then these are "terminal" points along each edge)
        //
        // As we collect the nodes (basically, edge points on the border).
        // we keep them in a sorted order in the above clockwise manner.
        var nodes = new SortedSet<BorderNode>(new BorderNodeComparer());
        bool hadLeftTop = false;
        bool hadTopRight = false;
        bool hadBottomRight = false;
        bool hadLeftBottom = false;
        for (int i = 0; i < edges.Count; i++)
        {
            var edge = edges[i];
            if (edge.Starter.DirectionOnBorder != Direction.None)
            {
                nodes.Add(new EdgeStartBorderNode(edge, i * 2));
                if (edge.Starter.DirectionOnBorder == Direction.LeftTop)
                    hadLeftTop = true;
                else if (edge.Starter.DirectionOnBorder == Direction.TopRight)
                    hadTopRight = true;
                else if (edge.Starter.DirectionOnBorder == Direction.BottomRight)
                    hadBottomRight = true;
                else if (edge.Starter.DirectionOnBorder == Direction.LeftBottom)
                    hadLeftBottom = true;
            }
            if (edge.Ender!.DirectionOnBorder != Direction.None)
            {
                nodes.Add(new EdgeEndBorderNode(edge, i * 2 + 1));
                if (edge.Ender.DirectionOnBorder == Direction.LeftTop)
                    hadLeftTop = true;
                else if (edge.Ender.DirectionOnBorder == Direction.TopRight)
                    hadTopRight = true;
                else if (edge.Ender.DirectionOnBorder == Direction.BottomRight)
                    hadBottomRight = true;
                else if (edge.Ender.DirectionOnBorder == Direction.LeftBottom)
                    hadLeftBottom = true;
            }
        }
        // If none of the edges hit any of the corners, then we need to add those as generic non-edge nodes
        if (!hadLeftTop)
            nodes.Add(new CornerBorderNode(new VoronoiVertex(minX, minY, Direction.LeftTop)));
        if (!hadTopRight)
            nodes.Add(new CornerBorderNode(new VoronoiVertex(maxX, minY, Direction.TopRight)));
        if (!hadBottomRight)
            nodes.Add(new CornerBorderNode(new VoronoiVertex(maxX, maxY, Direction.BottomRight)));
        if (!hadLeftBottom)
            nodes.Add(new CornerBorderNode(new VoronoiVertex(minX, maxY, Direction.LeftBottom)));
        EdgeBorderNode? previousEdgeNode = null;
        if (nodes.Min is EdgeBorderNode febn)
            previousEdgeNode = febn;
        if (previousEdgeNode == null)
        {
            foreach (BorderNode node in nodes.Reverse())
            {
                if (node is EdgeBorderNode rebn)
                {
                    previousEdgeNode = rebn;
                    break;
                }
            }
        }
        VoronoiCell? defaultCell = null;
        if (previousEdgeNode == null)
        {
            // We have no edges within bounds
            if (cells.Count != 0)
            {
                // But we may have site(s), so it's possible a site is in the bounds
                // (two sites couldn't be or there would be an edge)
                defaultCell = cells.FirstOrDefault(c =>
                    c.Site.X >= (minX)
                    && c.Site.X <= (maxX)
                    && c.Site.Y >= (minY)
                    && c.Site.Y <= (maxY)
                );
            }
        }
        // Edge tracking for neighbour recording
        VoronoiEdge firstEdge = null!; // to "loop" last edge back to first
        VoronoiEdge? previousEdge = null; // to connect each new edge to previous edg
        BorderNode? node2 = null; // i.e. last node
        foreach (var node in nodes)
        {
            var node1 = node2;
            node2 = node;
            if (node1 == null) // i.e. node == nodes.Min
                continue; // we are looking at first node, we will start from Min and next one
            var site =
                previousEdgeNode != null
                    ? previousEdgeNode is EdgeStartBorderNode
                        ? previousEdgeNode.Edge.Right
                        : previousEdgeNode.Edge.Left
                    : defaultCell;
            if (node1.Vertex != node2.Vertex)
            {
                var newEdge = new VoronoiEdge(
                    node1.Vertex,
                    node2.Vertex, // we are building these clockwise, so by definition the left side is out of bounds
                    site
                );
                // Record the first created edge for the last edge to "loop" around
                if (previousEdge is null)
                    firstEdge = newEdge;
                edges.Add(newEdge);
                site?.Edges.Add(newEdge);
                previousEdge = newEdge;
            }
            // Passing an edge node means that the site changes as we are now on the other side of this edge
            // (this doesn't happen in non-edge corner, which keep the same site)
            if (node is EdgeBorderNode cebn)
                previousEdgeNode = cebn;
        }
        var finalSite =
            previousEdgeNode != null
                ? previousEdgeNode is EdgeStartBorderNode
                    ? previousEdgeNode.Edge.Right
                    : previousEdgeNode.Edge.Left
                : defaultCell;
        var finalEdge = new VoronoiEdge(
            nodes.Max?.Vertex ?? throw VoronoiException.NullVertexOfBorderClosingNode(),
            nodes.Min?.Vertex ?? throw VoronoiException.NullVertexOfBorderClosingNode(), // we are building these clockwise, so by definition the left side is out of bounds
            finalSite
        );
        edges.Add(finalEdge);
        finalSite?.Edges.Add(finalEdge);
        return edges;
    }
}
