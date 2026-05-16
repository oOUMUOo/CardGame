using System;
using System.Collections.Generic;
using UnityEngine;

public static class DeckPresetSaveSystem
{
    public const int RequiredDeckSize = 15;
    private const string KeyBase = "StartingDeckNames";

    [Serializable]
    private class StartingDeckJson
    {
        public string[] names;
    }

    public static void ClearForActiveSlot()
    {
        string key = SaveSlotSystem.MakeSlotKey(KeyBase);
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    public static bool TryLoad(out string[] names)
    {
        names = null;
        string key = SaveSlotSystem.MakeSlotKey(KeyBase);
        if (!PlayerPrefs.HasKey(key))
        {
            return false;
        }

        var s = JsonUtility.FromJson<StartingDeckJson>(PlayerPrefs.GetString(key));
        if (s?.names == null || s.names.Length != RequiredDeckSize)
        {
            return false;
        }

        names = s.names;
        return true;
    }

    public static void Save(string[] names)
    {
        if (names == null || names.Length != RequiredDeckSize)
        {
            throw new ArgumentException($"Need exactly {RequiredDeckSize} cards.");
        }

        var s = new StartingDeckJson { names = names };
        PlayerPrefs.SetString(SaveSlotSystem.MakeSlotKey(KeyBase), JsonUtility.ToJson(s));
        PlayerPrefs.Save();
    }

    public static List<CardData> ResolveDeckOrFallback(DeckBuildCatalog catalog, HeroData hero)
    {
        if (catalog == null || hero == null)
        {
            return hero != null ? new List<CardData>(hero.Deck) : new List<CardData>();
        }

        if (!TryLoad(out var stored))
        {
            return new List<CardData>(hero.Deck);
        }

        var list = new List<CardData>(RequiredDeckSize);
        foreach (var name in stored)
        {
            if (!catalog.TryGetByAssetName(name, out var c))
            {
                return new List<CardData>(hero.Deck);
            }

            list.Add(c);
        }

        return list;
    }
}
