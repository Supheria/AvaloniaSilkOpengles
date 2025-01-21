using AltitudeMapGenerator.Extensions;
using AltitudeMapGenerator.VoronoiDiagram.Data;

namespace AltitudeMapGenerator.VoronoiDiagram.Model;

internal sealed class BeachSection(VoronoiCell cell)
{
    public VoronoiCell Cell { get; } = cell;

    public VoronoiEdge? Edge { get; set; } = null;

    //NOTE: this will change
    public FortuneCircleEvent? CircleEvent { get; set; } = null;
}

internal sealed class BeachLine
{
    RBTree<BeachSection> TheBeachLine { get; } = new();

    public void AddBeachSection(
        FortuneSiteEvent siteEvent,
        MinHeap<IFortuneEvent> eventQueue,
        HashSet<FortuneCircleEvent> deleted,
        LinkedList<VoronoiEdge> edges
    )
    {
        var cell = siteEvent.Cell;
        double x = cell.Site.X;
        double directrix = cell.Site.Y;

        RBTreeNode<BeachSection>? leftSection = null;
        RBTreeNode<BeachSection>? rightSection = null;
        RBTreeNode<BeachSection> node = TheBeachLine.Root;

        //find the parabola(s) above this site
        while (node != null && leftSection == null && rightSection == null)
        {
            double distanceLeft = LeftBreakpoint(node, directrix) - x;
            if (distanceLeft > 0)
            {
                //the new site is before the left breakpoint
                if (node.Left == null)
                {
                    // TODO: this is never covered by unit tests; need to figure out what triggers this and add a test, or if this is unreachable?
                    rightSection = node;
                }
                else
                {
                    node = node.Left;
                }
                continue;
            }

            double distanceRight = x - RightBreakpoint(node, directrix);
            if (distanceRight > 0)
            {
                //the new site is after the right breakpoint
                if (node.Right == null)
                {
                    leftSection = node;
                }
                else
                {
                    node = node.Right;
                }
                continue;
            }

            //the point lies below the left breakpoint
            if (distanceLeft.ApproxEqualTo(0))
            {
                leftSection = node.Previous;
                rightSection = node;
                continue;
            }

            //the point lies below the right breakpoint
            if (distanceRight.ApproxEqualTo(0))
            {
                leftSection = node;
                rightSection = node.Next;
                continue;
            }

            // distance Right < 0 and distance Left < 0
            // this section is above the new site
            leftSection = rightSection = node;
        }

        //our goal is to insert the new node between the
        //left and right sections
        var section = new BeachSection(cell);

        //left section could be null, in which case this node is the first
        //in the tree
        RBTreeNode<BeachSection> newSection = TheBeachLine.InsertSuccessor(leftSection, section);

        //new beach section is the first beach section to be added
        if (leftSection == null && rightSection == null)
        {
            return;
        }

        //main case:
        //if both left section and right section point to the same valid arc
        //we need to split the arc into a left arc and a right arc with our
        //new arc sitting in the middle
        if (leftSection != null && leftSection == rightSection)
        {
            //if the arc has a circle event, it was a false alarm.
            //remove it
            if (leftSection.Data.CircleEvent != null)
            {
                deleted.Add(leftSection.Data.CircleEvent);
                leftSection.Data.CircleEvent = null;
            }

            //we leave the existing arc as the left section in the tree
            //however we need to insert the right section defined by the arc
            var copy = new BeachSection(leftSection.Data.Cell);
            rightSection = TheBeachLine.InsertSuccessor(newSection, copy);

            //grab the projection of this site onto the parabola
            var site = leftSection.Data.Cell.Site;
            double y = ParabolaUtilities.EvalParabola(site.X, site.Y, directrix, x);
            var intersection = new VoronoiVertex(x, y);

            //create the two half edges corresponding to this intersection
            var leftEdge = new VoronoiEdge(intersection, cell, leftSection.Data.Cell);
            var rightEdge = new VoronoiEdge(intersection, leftSection.Data.Cell, cell);
            leftEdge.LastBeachLineNeighbor = rightEdge;

            //put the edge in the list
            edges.AddFirst(leftEdge);

            //store the left edge on each arc section
            newSection.Data.Edge = leftEdge;
            rightSection.Data.Edge = rightEdge;

            //store neighbors for delaunay
            leftSection.Data.Cell.Neighbours.Add(newSection.Data.Cell);
            newSection.Data.Cell.Neighbours.Add(leftSection.Data.Cell);

            //create circle events
            CheckCircle(leftSection, eventQueue);
            CheckCircle(rightSection, eventQueue);
        }
        //site is the last beach section on the beach line
        //this can only happen if all previous sites
        //had the same y value
        else if (leftSection != null && rightSection == null)
        {
            var site = leftSection.Data.Cell.Site;
            var start = new VoronoiVertex((site.X + cell.Site.X) / 2, double.MinValue);
            var infEdge = new VoronoiEdge(start, leftSection.Data.Cell, cell);
            var newEdge = new VoronoiEdge(start, cell, leftSection.Data.Cell)
            {
                LastBeachLineNeighbor = infEdge,
            };
            edges.AddFirst(newEdge);

            leftSection.Data.Cell.Neighbours.Add(newSection.Data.Cell);
            newSection.Data.Cell.Neighbours.Add(leftSection.Data.Cell);

            newSection.Data.Edge = newEdge;

            //cant check circles since they are colinear
        }
        //site is directly above a break point
        else if (leftSection != null && leftSection != rightSection)
        {
            //remove false alarms
            if (leftSection.Data.CircleEvent != null)
            {
                deleted.Add(leftSection.Data.CircleEvent);
                leftSection.Data.CircleEvent = null;
            }

            if (rightSection.Data.CircleEvent != null)
            {
                deleted.Add(rightSection.Data.CircleEvent);
                rightSection.Data.CircleEvent = null;
            }

            //the breakpoint will dissapear if we add this site
            //which means we will create an edge
            //we treat this very similar to a circle event since
            //an edge is finishing at the center of the circle
            //created by circumscribing the left center and right
            //sites

            //bring a to the origin
            VoronoiCell leftCell = leftSection.Data.Cell;
            double ax = leftCell.Site.X;
            double ay = leftCell.Site.Y;
            double bx = cell.Site.X - ax;
            double by = cell.Site.Y - ay;

            VoronoiCell rightCell = rightSection.Data.Cell;
            double cx = rightCell.Site.X - ax;
            double cy = rightCell.Site.Y - ay;
            double d = bx * cy - by * cx;
            double magnitudeB = bx * bx + by * by;
            double magnitudeC = cx * cx + cy * cy;
            VoronoiVertex vertex = new VoronoiVertex(
                (cy * magnitudeB - by * magnitudeC) / (2 * d) + ax,
                (bx * magnitudeC - cx * magnitudeB) / (2 * d) + ay
            );

            // If the edge ends up being 0 length (i.e. start and end are the same point),
            // then this is a location with 4+ equidistant sites.
            if (rightSection.Data.Edge.Starter == vertex) // i.e. what we would set as .End
            {
                // Reuse vertex (or we will have 2 ongoing points at the same location)
                vertex = rightSection.Data.Edge.Starter;

                // Discard the edge
                edges.Remove(rightSection.Data.Edge);

                // Disconnect (delaunay) neighbours
                leftCell.Neighbours.Remove(rightCell);
                rightCell.Neighbours.Remove(leftCell);
            }
            else
            {
                rightSection.Data.Edge.Ender = vertex;
            }

            //next we create a two new edges
            newSection.Data.Edge = new VoronoiEdge(vertex, cell, leftSection.Data.Cell);
            rightSection.Data.Edge = new VoronoiEdge(vertex, rightSection.Data.Cell, cell);

            edges.AddFirst(newSection.Data.Edge);
            edges.AddFirst(rightSection.Data.Edge);

            //add neighbors for delaunay
            newSection.Data.Cell.Neighbours.Add(leftSection.Data.Cell);
            leftSection.Data.Cell.Neighbours.Add(newSection.Data.Cell);

            newSection.Data.Cell.Neighbours.Add(rightSection.Data.Cell);
            rightSection.Data.Cell.Neighbours.Add(newSection.Data.Cell);

            CheckCircle(leftSection, eventQueue);
            CheckCircle(rightSection, eventQueue);
        }
    }

    public void RemoveBeachSection(
        FortuneCircleEvent circle,
        MinHeap<IFortuneEvent> eventQueue,
        HashSet<FortuneCircleEvent> deleted,
        LinkedList<VoronoiEdge> edges
    )
    {
        RBTreeNode<BeachSection> section = circle.ToDelete;
        double x = circle.X;
        double y = circle.YCenter;
        VoronoiVertex vertex = new VoronoiVertex(x, y);

        //multiple edges could end here
        List<RBTreeNode<BeachSection>> toBeRemoved = new List<RBTreeNode<BeachSection>>();

        //look left
        RBTreeNode<BeachSection> prev = section.Previous;
        while (
            prev.Data.CircleEvent != null
            && x.ApproxEqualTo(prev.Data.CircleEvent.X)
            && y.ApproxEqualTo(prev.Data.CircleEvent.YCenter)
        )
        {
            toBeRemoved.Add(prev);
            prev = prev.Previous;
        }

        RBTreeNode<BeachSection> next = section.Next;
        while (
            next.Data.CircleEvent != null
            && x.ApproxEqualTo(next.Data.CircleEvent.X)
            && y.ApproxEqualTo(next.Data.CircleEvent.YCenter)
        )
        {
            toBeRemoved.Add(next);
            next = next.Next;
        }

        section.Data.Edge.Ender = vertex;
        section.Next.Data.Edge.Ender = vertex;
        section.Data.CircleEvent = null;

        //odds are this double writes a few edges but this is clean...
        foreach (RBTreeNode<BeachSection> remove in toBeRemoved)
        {
            remove.Data.Edge.Ender = vertex;
            remove.Next.Data.Edge.Ender = vertex;
            deleted.Add(remove.Data.CircleEvent);
            remove.Data.CircleEvent = null;
        }

        //need to delete all upcoming circle events with this node
        if (prev.Data.CircleEvent != null)
        {
            deleted.Add(prev.Data.CircleEvent);
            prev.Data.CircleEvent = null;
        }
        if (next.Data.CircleEvent != null)
        {
            deleted.Add(next.Data.CircleEvent);
            next.Data.CircleEvent = null;
        }

        //create a new edge with start point at the vertex and assign it to next
        VoronoiEdge newEdge = new VoronoiEdge(vertex, next.Data.Cell, prev.Data.Cell);
        next.Data.Edge = newEdge;
        edges.AddFirst(newEdge);

        //add neighbors for delaunay
        prev.Data.Cell.Neighbours.Add(next.Data.Cell);
        next.Data.Cell.Neighbours.Add(prev.Data.Cell);

        //remove the sectionfrom the tree
        TheBeachLine.RemoveNode(section);
        foreach (RBTreeNode<BeachSection> remove in toBeRemoved)
        {
            TheBeachLine.RemoveNode(remove);
        }

        CheckCircle(prev, eventQueue);
        CheckCircle(next, eventQueue);
    }

    private static double LeftBreakpoint(RBTreeNode<BeachSection> node, double directrix)
    {
        var leftNode = node.Previous;
        //degenerate parabola
        var site = node.Data.Cell.Site;
        if ((site.Y - directrix).ApproxEqualTo(0))
            return site.X;
        //node is the first piece of the beach line
        if (leftNode == null)
            return double.NegativeInfinity;
        //left node is degenerate
        var leftSite = leftNode.Data.Cell.Site;
        if ((leftSite.Y - directrix).ApproxEqualTo(0))
            return leftSite.X;
        return ParabolaUtilities.IntersectParabolaX(
            leftSite.X,
            leftSite.Y,
            site.X,
            site.Y,
            directrix
        );
    }

    private static double RightBreakpoint(RBTreeNode<BeachSection> node, double directrix)
    {
        var rightNode = node.Next;
        //degenerate parabola
        var site = node.Data.Cell.Site;
        if ((site.Y - directrix).ApproxEqualTo(0))
            return site.X;
        //node is the last piece of the beach line
        if (rightNode == null)
            return double.PositiveInfinity;
        //left node is degenerate
        var rightSite = rightNode.Data.Cell.Site;
        if ((rightSite.Y - directrix).ApproxEqualTo(0))
            return rightSite.X;
        return ParabolaUtilities.IntersectParabolaX(
            site.X,
            site.Y,
            rightSite.X,
            rightSite.Y,
            directrix
        );
    }

    private static void CheckCircle(
        RBTreeNode<BeachSection> section,
        MinHeap<IFortuneEvent> eventQueue
    )
    {
        //if (section == null)
        //    return;
        RBTreeNode<BeachSection> left = section.Previous;
        RBTreeNode<BeachSection> right = section.Next;
        if (left == null || right == null)
            return;

        var leftCell = left.Data.Cell;
        var centerCell = section.Data.Cell;
        var rightCell = right.Data.Cell;

        //if the left arc and right arc are defined by the same
        //focus, the two arcs cannot converge
        if (leftCell == rightCell)
        {
            // TODO: this is never covered by unit tests; need to figure out what triggers this and add a test, or if this is unreachable?
            return;
        }
        //
        //MATH HACKS: place center at origin and
        //draw vectors a and c to
        //left and right respectively
        double bx = centerCell.Site.X,
            by = centerCell.Site.Y,
            ax = leftCell.Site.X - bx,
            ay = leftCell.Site.Y - by,
            cx = rightCell.Site.X - bx,
            cy = rightCell.Site.Y - by;
        //
        //The center beach section can only dissapear when
        //the angle between a and c is negative
        double d = ax * cy - ay * cx;
        if (d.ApproxGreaterThanOrEqualTo(0))
            return;
        double magnitudeA = ax * ax + ay * ay;
        double magnitudeC = cx * cx + cy * cy;
        double x = (cy * magnitudeA - ay * magnitudeC) / (2 * d);
        double y = (ax * magnitudeC - cx * magnitudeA) / (2 * d);
        //add back offset
        double ycenter = y + by;
        //y center is off
        FortuneCircleEvent circleEvent = new FortuneCircleEvent(
            new VoronoiVertex(x + bx, ycenter + Math.Sqrt(x * x + y * y)),
            ycenter,
            section
        );
        section.Data.CircleEvent = circleEvent;
        eventQueue.Insert(circleEvent);
    }
}
