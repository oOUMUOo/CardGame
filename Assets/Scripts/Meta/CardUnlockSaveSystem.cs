using System;
using System.Collections.Generic;
using UnityEngine;

public static class CardUnlockSaveSystem
{
    private const string Key = "Meta_UnlockedCards_v1";

    [Serializable]
    private class StringListSave
    {
        public string[] items;
    }

    public static void EnsureSeededFromCatalog(DeckBuildCatalog catalog)
    {
        if (catalog == null || PlayerPrefs.HasKey(Key))
        {
            return;
        }

        var names = new List<string>();
        foreach (var c in catalog.Cards)
        {
            if (c != null)
            {
                names.Add(c.name);
            }
        }

        PlayerPrefs.SetString(Key, JsonUtility.ToJson(new StringListSave { items = names.ToArray() }));
        PlayerPrefs.Save();
    }

    public static List<CardData> GetUnlockedCardsSorted(DeckBuildCatalog catalog)
    {
        if (catalog == null)
        {
            return new List<CardData>();
        }

        EnsureSeededFromCatalog(catalog);

        var allowed = new HashSet<string>();
        if (PlayerPrefs.HasKey(Key))
        {
            var parsed = JsonUtility.FromJson<StringListSave>(PlayerPrefs.GetString(Key));
            if (parsed?.items != null)
            {
                foreach (var n in parsed.items)
                {
                    if (!string.IsNullOrEmpty(n))
                    {
                        allowed.Add(n);
                    }
                }
            }
        }

        var result = new List<CardData>();
        foreach (var c in catalog.Cards)
        {
            if (c == null)
            {
                continue;
            }

            if (allowed.Count == 0 || allowed.Contains(c.name))
            {
                result.Add(c);
            }
        }

        if (result.Count == 0)
        {
            foreach (var c in catalog.Cards)
            {
                if (c != null)
                {
                    result.Add(c);
                }
            }
        }

        result.Sort((a, b) => string.CompareOrdinal(a.Title, b.Title));
        return result;
    }
}
