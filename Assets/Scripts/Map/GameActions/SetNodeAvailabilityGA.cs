using System.Collections.Generic;
using UnityEngine;

public class SetNodeAvailabilityGA : GameAction
{
    public List<MapNode> AvailableNodes { get; private set; }
    
    public SetNodeAvailabilityGA(List<MapNode> availableNodes)
    {
        AvailableNodes = availableNodes;
    }
}
