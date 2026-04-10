using UnityEngine;

public class EnterNodeGA : GameAction
{
    public MapNode MapNode { get; private set; }
    
    public EnterNodeGA(MapNode mapNode)
    {
        MapNode = mapNode;
    }
}
