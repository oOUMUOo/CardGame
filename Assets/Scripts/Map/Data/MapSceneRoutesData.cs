using UnityEngine;

[CreateAssetMenu(menuName = "Data/Map Scene Routes")]
public class MapSceneRoutesData : ScriptableObject
{
    [field: SerializeField] public string MapSceneName { get; private set; } = "MapScene";
    [field: SerializeField] public string CombatSceneName { get; private set; } = "CombatScene";
    [field: SerializeField] public string EliteCombatSceneName { get; private set; } = "CombatScene";
    [field: SerializeField] public string BossCombatSceneName { get; private set; } = "CombatScene";
    [field: SerializeField] public string MerchantSceneName { get; private set; } = "MerchantScene";
    [field: SerializeField] public string TreasureSceneName { get; private set; } = "TreasureScene";
    [field: SerializeField] public string EventSceneName { get; private set; } = "EventScene";
    [field: SerializeField] public string RestSceneName { get; private set; } = "RestScene";
}
