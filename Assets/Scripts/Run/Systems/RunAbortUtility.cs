using UnityEngine;
using UnityEngine.SceneManagement;

public static class RunAbortUtility
{
    public static void ClearRunSavableProgress()
    {
        RunSaveSystem.ClearHeroHealth();
        RunSaveSystem.ClearGold();
        RunRareOffsetSystem.Clear();
        RunDeckBonusCardsSystem.Clear();
        MapSaveSystem.ClearSave();
    }

    public static bool TryQuitRunAndLoadScene(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"RunAbortUtility: scene '{sceneName}' is not available in Build Settings.");
            return false;
        }

        ClearRunSavableProgress();
        SceneManager.LoadScene(sceneName);
        return true;
    }
}
