namespace AltitudeMapGenerator.VoronoiDiagram;

internal sealed class VoronoiException(string message) : Exception(message)
{
    public static void ThrowIfCountZero<T>(List<T> items, string name)
    {
        if (items.Count is 0)
            throw new VoronoiException($"{name} is empty");
    }

    public static VoronoiException EmptyMinHeap()
    {
        return new($"Min heap is empty");
    }

    public static VoronoiException NullVertexOfBorderClosingNode()
    {
        return new($"vertex of node in border closing is null");
    }

    public static VoronoiException NullFortuneCircleEvent(string name)
    {
        return new($"{name} is null fortune circle event");
    }

    public static VoronoiException NullFortuneSiteEvent(string name)
    {
        return new($"{name} is null fortune circle event");
    }

    public static VoronoiException NoMatchVertexInDijkstra(string name)
    {
        return new($"there is no match to {name} in vertex list of dijkstra");
    }
}
