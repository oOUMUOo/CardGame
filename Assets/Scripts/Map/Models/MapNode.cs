using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public NodeBlueprintData Blueprint { get; private set; }
    public MapNodeType NodeType => Blueprint.NodeType;
    public Vector3 Position { get; set; }
    public int LayerIndex { get; private set; } = -1;
    public List<MapNode> ConnectedNodes { get; private set; } = new();
    public bool IsAvailable { get; private set; }
    public bool IsVisited { get; private set; }
    public bool IsCurrent { get; private set; }

    public MapNode(NodeBlueprintData blueprint)
    {
        Blueprint = blueprint;
    }

    public void SetLayerIndex(int layerIndex)
    {
        LayerIndex = layerIndex;
    }

    public void SetAvailable(bool available)
    {
        IsAvailable = available;
    }

    public void SetVisited(bool visited)
    {
        IsVisited = visited;
    }

    public void SetCurrent(bool current)
    {
        IsCurrent = current;
    }

    public void AddConnection(MapNode node)
    {
        if (!ConnectedNodes.Contains(node))
        {
            ConnectedNodes.Add(node);
        }
    }
}
