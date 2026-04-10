using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public List<List<MapNode>> Layers { get; private set; } = new();
    public MapData Data { get; private set; }
    public int GenerationSeed { get; private set; }

    public Map(MapData mapData, int generationSeed)
    {
        Data = mapData;
        GenerationSeed = generationSeed;
    }

    public void AddLayer(List<MapNode> layer)
    {
        Layers.Add(layer);
    }

    public MapNode GetNode(int layerIndex, int nodeIndex)
    {
        if (layerIndex >= 0 && layerIndex < Layers.Count &&
            nodeIndex >= 0 && nodeIndex < Layers[layerIndex].Count)
        {
            return Layers[layerIndex][nodeIndex];
        }

        return null;
    }

    public int LayerCount => Layers.Count;
}
