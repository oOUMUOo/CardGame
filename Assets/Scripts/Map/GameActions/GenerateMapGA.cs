using UnityEngine;

public class GenerateMapGA : GameAction
{
    public MapData MapData { get; private set; }
    public int GenerationSeed { get; private set; }
    public MapSaveData RestoreFromSave { get; private set; }

    public GenerateMapGA(MapData mapData, int? generationSeed = null, MapSaveData restoreFromSave = null)
    {
        MapData = mapData;
        GenerationSeed = generationSeed ?? Random.Range(int.MinValue, int.MaxValue);
        RestoreFromSave = restoreFromSave;
    }
}
