using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Map")]
public class MapData : ScriptableObject
{
    [field: SerializeField] public List<NodeBlueprintData> NodeBlueprints { get; private set; }
    [field: SerializeField] public int PreBossNodesMin { get; private set; } = 4;
    [field: SerializeField] public int PreBossNodesMax { get; private set; } = 5;
    [field: SerializeField] public int StartingNodesMin { get; private set; } = 3;
    [field: SerializeField] public int StartingNodesMax { get; private set; } = 4;
    [field: SerializeField] public List<MapLayerData> Layers { get; private set; }
    [field: SerializeField] public MapOrientation Orientation { get; private set; } = MapOrientation.BOTTOM_TO_TOP;
    [field: SerializeField] public float NodeDistance { get; private set; } = 3f;
    [field: SerializeField] public float PositionRandomization { get; private set; } = 0.72f;
    [field: SerializeField] public float NodeTypeRandomization { get; private set; } = 0.72f;
    [field: SerializeField] public int ExtraPaths { get; private set; } = 0;
}