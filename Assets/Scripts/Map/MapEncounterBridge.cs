using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Вызывайте из сцен боя/событий после успешного завершения узла, затем возвращайтесь на карту.
/// </summary>
public static class MapEncounterBridge
{
    public static void CompleteEncounterAndLoadMap(MapSceneRoutesData routes)
    {
        if (routes == null)
        {
            Debug.LogError("MapEncounterBridge: MapSceneRoutesData is not assigned.");
            return;
        }

        MapSaveSystem.RecordEncounterCompleted();
        SceneManager.LoadScene(routes.MapSceneName);
    }

    public static void CompleteEncounterAndLoadMap(string mapSceneName)
    {
        MapSaveSystem.RecordEncounterCompleted();
        SceneManager.LoadScene(mapSceneName);
    }
}
