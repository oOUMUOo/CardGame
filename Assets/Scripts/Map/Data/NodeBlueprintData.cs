using UnityEngine;

[CreateAssetMenu(menuName = "Data/NodeBlueprint")]
public class NodeBlueprintData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public MapNodeType NodeType { get; private set; }
}