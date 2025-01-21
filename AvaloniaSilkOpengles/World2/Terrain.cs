using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using AltitudeMapGenerator;
using AltitudeMapGenerator.Layout;
using Avalonia;
using Avalonia.Platform;
using AvaloniaSilkOpengles.Assets;
using AvaloniaSilkOpengles.Graphics;
using LocalUtilities.SimpleScript;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World2;

public class Terrain : RenderableObject
{
    Dictionary<(float X, float Z), double> AltitudeMap { get; } = [];
    List<AltitudeSite> AltitudeSites { get; } = [];
    uint IndexHead { get; set; }

    public Terrain(GL gl)
    {
        var rawMap = LoadRawMap();
        if (rawMap is null)
            return;
        GenerateAltitudeMap(rawMap);
        GenerateFaces();
        Mesh = new(gl, Vertices, Indices, []);
    }

    private static AltitudeMap? LoadRawMap()
    {
        var uri = AssetsRead.GenerateUri("Test", "test altitude map.txt");
        using var stream = AssetLoader.Open(uri);
        using var memoy = new MemoryStream();
        stream.CopyTo(memoy);
        var data = memoy.ToArray();
        return SerializeTool.Deserialize<AltitudeMap>(data, 0, data.Length);
    }

    private void GenerateAltitudeMap(AltitudeMap rawMap)
    {
        var altitudes = new Dictionary<(float X, float Z), List<double>>();
        var random = new Random();
        var map = new Dictionary<PixelPoint, AltitudePoint>();
        rawMap.AltitudePoints.ForEach(p => map[p.Site] = p);
        for (var i = 0; i < rawMap.Width; i++)
        {
            for (var j = 0; j < rawMap.Height; j++)
            {
                AltitudePoint point;
                if (map.TryGetValue(new(i, j), out var p))
                    point = p;
                else
                    point = new(){Site = new(i,j), Altitude = 0f};
                
                var terrainType = GetTerrainType(point.Altitude / rawMap.AltitudeMax, random.NextDouble());
                var site = new AltitudeSite(point.Site, terrainType);
                AltitudeSites.Add(site);
                foreach (var xz in site.VertexXzs.Values)
                {
                    if (altitudes.TryGetValue(xz, out var altitude))
                        altitude.Add(point.Altitude);
                    else
                        altitudes[xz] = [point.Altitude];
                }
            }
        }
        // foreach (var point in altitudePoints)
        // {
        //     var terrainType = GetTerrainType(point.Altitude / rawMap.AltitudeMax, random.Next());
        //     var site = new AltitudeSite(point.Site, terrainType);
        //     AltitudeSites.Add(site);
        //     foreach (var xz in site.VertexXzs.Values)
        //     {
        //         if (altitudes.TryGetValue(xz, out var altitude))
        //             altitude.Add(point.Altitude);
        //         else
        //             altitudes[xz] = [point.Altitude];
        //     }
        // }
        foreach (var (key, value) in altitudes)
            AltitudeMap[key] = value.Average();
    }

    private void GenerateFaces()
    {
        foreach (var site in AltitudeSites)
        {
            var v0Altitude = GetV0Altitude(site);
            AltitudeMap[site.VertexXzs[VertexIndex.V0]] = v0Altitude;
            var faces = GetSiteFaces(site);
            foreach (var face in faces)
                AddFace(face);
        }
    }
    
    private double GetV0Altitude(AltitudeSite site)
    {
        var altitudes = new List<double>(4)
        {
            AltitudeMap[site.VertexXzs[VertexIndex.V1]],
            AltitudeMap[site.VertexXzs[VertexIndex.V2]],
            AltitudeMap[site.VertexXzs[VertexIndex.V3]],
            AltitudeMap[site.VertexXzs[VertexIndex.V4]],
        };
        return altitudes.Average();
    }

    private void AddFace(Face face)
    {
        Vertices.AddRange(face.Vertices);
        Indices.AddRange([IndexHead++, IndexHead++, IndexHead++]);
    }

    private TerrainType GetTerrainType(double altitudeRatio, double random)
    {
        switch (altitudeRatio)
        {
            case < 0.05:
            {
                // return TerrainType.Plain;
                if (random < (0.7))
                    return TerrainType.Plain;
                if (random < (0.9))
                    return TerrainType.Wood;
                break;
            }
            case < 0.2:
            {
                // return TerrainType.Wood;
                if (random < (0.8))
                    return TerrainType.Wood;
                if (random < (0.95))
                    return TerrainType.Hill;
                break;
            }
            default:
            {
                // return TerrainType.Hill;
                if (random < (0.8))
                    return TerrainType.Hill;
                if (random < (0.99))
                    return TerrainType.Wood;
                break;
            }
        }

        return TerrainType.Stream;
    }

    private List<Face> GetSiteFaces(AltitudeSite site)
    {
        var c0 = GetCoord(site, VertexIndex.V0);
        var c1 = GetCoord(site, VertexIndex.V1);
        var c2 = GetCoord(site, VertexIndex.V2);
        var c3 = GetCoord(site, VertexIndex.V3);
        var c4 = GetCoord(site, VertexIndex.V4);
        return
        [
            GetFace(site.TerrainType, FaceType.Back, c0, c1, c2, c3, c4),
            GetFace(site.TerrainType, FaceType.Left, c0, c1, c2, c3, c4),
            GetFace(site.TerrainType, FaceType.Front, c0, c1, c2, c3, c4),
            GetFace(site.TerrainType, FaceType.Right, c0, c1, c2, c3, c4),
        ];
    }

    private Vector3 GetCoord(AltitudeSite altitudeSite, VertexIndex index)
    {
        var (x, z) = altitudeSite.VertexXzs[index];
        var y = (float)AltitudeMap[(x, z)] * 0.25f;
        return new(x, y, z);
    }

    private Face GetFace(
        TerrainType terrainType,
        FaceType faceType,
        Vector3 c0,
        Vector3 c1,
        Vector3 c2,
        Vector3 c3,
        Vector3 c4
    )
    {
        Vector3[] coords = faceType switch
        {
            FaceType.Back => [c1, c0, c4],
            FaceType.Left => [c2, c0, c1],
            FaceType.Front => [c3, c0, c2],
            FaceType.Right => [c4, c0, c3],
            _ => throw new ArgumentOutOfRangeException(nameof(faceType), faceType, null),
        };
        var color = terrainType switch
        {
            TerrainType.None => NormalizeColor(Color.Black),
            TerrainType.Plain => NormalizeColor(Color.LightYellow),
            TerrainType.Stream => NormalizeColor(Color.SkyBlue),
            TerrainType.Wood => NormalizeColor(Color.LimeGreen),
            TerrainType.Hill => NormalizeColor(Color.DarkSlateGray),
        };
        return new(coords, color);
    }

    private static Vector3 NormalizeColor(Color color)
    {
        return new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
    }
}
