using System;
using System.Collections.Generic;
using UnityEngine;

public static class RunDeckBonusCardsSystem
{
    private const string KeyBase = "RunDeckBonusCardNames";

    [Serializable]
    private class NamesJson
    {
        public string[] items;
    }

    private static string Key => SaveSlotSystem.MakeSlotKey(KeyBase);

    public static IReadOnlyList<string> GetNames()
    {
        if (!PlayerPrefs.HasKey(Key))
        {
            return Array.Empty<string>();
        }

        var parsed = JsonUtility.FromJson<NamesJson>(PlayerPrefs.GetString(Key));
        if (parsed?.items == null || parsed.items.Length == 0)
        {
            return Array.Empty<string>();
        }

        return parsed.items;
    }

    public static void AddCard(string cardAssetName)
    {
        if (string.IsNullOrEmpty(cardAssetName))
        {
            return;
        }

        var list = new List<string>(GetNames());
        list.Add(cardAssetName);
        Save(list);
    }

    public static void Clear()
    {
        if (PlayerPrefs.HasKey(Key))
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
        }
    }

    private static void Save(List<string> names)
    {
        var json = JsonUtility.ToJson(new NamesJson { items = names.ToArray() });
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }
}
