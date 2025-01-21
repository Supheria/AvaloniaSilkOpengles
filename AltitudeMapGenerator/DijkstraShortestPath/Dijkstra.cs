using AltitudeMapGenerator.VoronoiDiagram;
using Avalonia;

namespace AltitudeMapGenerator.DijkstraShortestPath;

internal sealed class Dijkstra
{
    double[,] Graph { get; }

    List<PixelEdge> Edges { get; }

    List<DijkstraNode> Nodes { get; }

    internal List<PixelEdge>? Path { get; private set; }

    internal Dijkstra(
        List<PixelEdge> edges,
        List<PixelPoint> vertexes,
        PixelPoint startVertex,
        PixelPoint finishVertex
    )
    {
        Edges = edges;
        Nodes = [];
        Graph = new double[vertexes.Count, vertexes.Count];
        foreach (var row in Enumerable.Range(0, vertexes.Count))
        {
            var rowNode = vertexes[row];
            foreach (var colnum in Enumerable.Range(0, vertexes.Count))
            {
                if (row == colnum)
                {
                    Graph[row, colnum] = 0;
                    continue;
                }
                var edge = Edges.FirstOrDefault(x =>
                    x.Starter == rowNode && x.Ender == vertexes[colnum]
                );
                Graph[row, colnum] = edge == null ? double.MaxValue : edge.Length;
            }
            Nodes.Add(new(vertexes[row], row));
        }
        Path = GetPath(startVertex, finishVertex);
    }

    private List<PixelEdge>? GetPath(PixelPoint startVertex, PixelPoint endVertex)
    {
        VoronoiException.ThrowIfCountZero(Nodes, "dijkstra nodes");
        Nodes.First(n => n.PixelPoint == startVertex).Used = true;
        Nodes.ForEach(x =>
        {
            var index = 0;
            while (index < Nodes.Count)
            {
                if (startVertex == Nodes[index].PixelPoint)
                    break;
                index++;
            }
            x.Weight = GetRowArray(index)[x.Index];
            x.Nodes.Add(startVertex);
        });
        while (Nodes.Any(x => !x.Used))
        {
            var item = GetUnUsedAndMinNodeItem();
            if (item == null)
                break;
            item.Used = true;
            var tempRow = GetRowArray(item.Index);
            foreach (var nodeItem in Nodes)
            {
                if (nodeItem.Weight > tempRow[nodeItem.Index] + item.Weight)
                {
                    nodeItem.Weight = tempRow[nodeItem.Index] + item.Weight;
                    nodeItem.Nodes.Clear();
                    nodeItem.Nodes.AddRange(item.Nodes);
                    nodeItem.Nodes.Add(item.PixelPoint);
                }
            }
        }
        var desNodeitem = Nodes.First(x => x.PixelPoint == endVertex);
        if (!(desNodeitem.Used && desNodeitem.Weight < double.MaxValue))
            return null;
        var path = new List<PixelEdge>();
        foreach (var index in Enumerable.Range(0, desNodeitem.Nodes.Count - 1))
        {
            var e = Edges.FirstOrDefault(e =>
                e.Starter == desNodeitem.Nodes[index] && e.Ender == desNodeitem.Nodes[index + 1]
            );
            if (e is not null)
                path.Add(e);
        }
        var edge = Edges.FirstOrDefault(x =>
            x.Starter == desNodeitem.Nodes.Last() && x.Ender == endVertex
        );
        if (edge is not null)
            path.Add(edge);
        return path;
    }

    private DijkstraNode? GetUnUsedAndMinNodeItem()
    {
        return Nodes
            .Where(x => !x.Used && x.Weight != double.MaxValue)
            .OrderBy(x => x.Weight)
            .FirstOrDefault();
    }

    private double[] GetRowArray(int row)
    {
        double[] result = new double[Graph.GetLength(1)];
        foreach (var index in Enumerable.Range(0, result.Length))
        {
            result[index] = Graph[row, index];
        }
        return result;
    }
}
