using UnityEngine;

public class MapSetupSystem : MonoBehaviour
{
    [SerializeField] private MapData mapData;
    [SerializeField] private bool continueLastRun;

    private void Start()
    {
        if (continueLastRun && MapSaveSystem.TryLoad(out MapSaveData saveData))
        {
            GenerateMapGA generateMapGA = new(mapData, saveData.GenerationSeed, saveData);
            ActionSystem.Instance.Perform(generateMapGA);
        }
        else
        {
            MapSaveSystem.ClearSave();
            RunSaveSystem.ClearHeroHealth();
            RunSaveSystem.ClearGold();
            RunRareOffsetSystem.Clear();
            RunDeckBonusCardsSystem.Clear();
            GenerateMapGA generateMapGA = new(mapData);
            ActionSystem.Instance.Perform(generateMapGA);
        }
    }
}
