using UnityEngine;

public static class RunSaveSystem
{
    private const string HeroHealthBaseKey = "RunHeroHealth";
    private const string GoldBaseKey = "RunGold";
    private static string HeroHealthKey => SaveSlotSystem.MakeSlotKey(HeroHealthBaseKey);
    private static string GoldKey => SaveSlotSystem.MakeSlotKey(GoldBaseKey);

    public static bool TryLoadHeroHealth(out int health)
    {
        health = 0;
        if (!PlayerPrefs.HasKey(HeroHealthKey))
        {
            return false;
        }

        health = PlayerPrefs.GetInt(HeroHealthKey);
        return true;
    }

    public static void SaveHeroHealth(int health)
    {
        PlayerPrefs.SetInt(HeroHealthKey, health);
        PlayerPrefs.Save();
    }

    public static void ClearHeroHealth()
    {
        if (!PlayerPrefs.HasKey(HeroHealthKey))
        {
            return;
        }

        PlayerPrefs.DeleteKey(HeroHealthKey);
        PlayerPrefs.Save();
    }

    public static int LoadGold()
    {
        return Mathf.Max(0, PlayerPrefs.GetInt(GoldKey, 0));
    }

    public static void SaveGold(int gold)
    {
        PlayerPrefs.SetInt(GoldKey, Mathf.Max(0, gold));
        PlayerPrefs.Save();
    }

    public static void AddGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        SaveGold(LoadGold() + amount);
    }

    public static bool TrySpendGold(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        int current = LoadGold();
        if (current < amount)
        {
            return false;
        }

        SaveGold(current - amount);
        return true;
    }

    public static void ClearGold()
    {
        if (!PlayerPrefs.HasKey(GoldKey))
        {
            return;
        }

        PlayerPrefs.DeleteKey(GoldKey);
        PlayerPrefs.Save();
    }
}
