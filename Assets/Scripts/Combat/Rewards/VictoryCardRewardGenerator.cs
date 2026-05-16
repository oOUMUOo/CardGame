using System.Collections.Generic;
using UnityEngine;

public static class VictoryCardRewardGenerator
{
    public static List<CardData> GenerateThreeRewards(MapNodeType encounterType, DeckBuildCatalog catalog)
    {
        return GenerateRewardOffers(encounterType, catalog, 3);
    }

    public static List<CardData> GenerateRewardOffers(MapNodeType encounterType, DeckBuildCatalog catalog, int count)
    {
        var result = new List<CardData>(Mathf.Max(0, count));
        if (catalog == null || count <= 0)
        {
            return result;
        }

        var usedNames = new HashSet<string>();
        int offset = RunRareOffsetSystem.GetOffset();

        for (var i = 0; i < count; i++)
        {
            CardRarity rolled = RollRarity(encounterType, ref offset);
            CardData picked = PickCard(catalog, rolled, usedNames);
            if (picked != null)
            {
                usedNames.Add(picked.name);
                result.Add(picked);
            }

            RunRareOffsetSystem.SetOffset(offset);
        }

        return result;
    }

    private static CardRarity RollRarity(MapNodeType encounterType, ref int rareOffset)
    {
        rareOffset = Mathf.Clamp(rareOffset, RunRareOffsetSystem.MinOffset, RunRareOffsetSystem.MaxOffset);

        if (encounterType == MapNodeType.BOSS)
        {
            rareOffset = RunRareOffsetSystem.MinOffset;
            return CardRarity.Rare;
        }

        GetBaseWeights(encounterType, out float baseC, out float baseU, out float baseR);

        float rTarget = Mathf.Clamp(baseR + rareOffset, 0f, 100f);
        float delta = rTarget - baseR;

        float newC;
        float newU;
        float newR;

        if (delta >= 0f)
        {
            float takeC = Mathf.Min(baseC, delta);
            newC = baseC - takeC;
            float rem = delta - takeC;
            float takeU = Mathf.Min(baseU, rem);
            newU = baseU - takeU;
            newR = baseR + takeC + takeU;
        }
        else
        {
            float give = -delta;
            newC = baseC + give;
            newU = baseU;
            newR = baseR + delta;
        }

        float sum = newC + newU + newR;
        if (sum <= 0f)
        {
            rareOffset = RunRareOffsetSystem.MinOffset;
            return CardRarity.Rare;
        }

        float t = Random.Range(0f, sum);
        if (t < newC)
        {
            rareOffset = Mathf.Clamp(rareOffset + 1, RunRareOffsetSystem.MinOffset, RunRareOffsetSystem.MaxOffset);
            return CardRarity.Common;
        }

        if (t < newC + newU)
        {
            return CardRarity.Uncommon;
        }

        rareOffset = RunRareOffsetSystem.MinOffset;
        return CardRarity.Rare;
    }

    private static void GetBaseWeights(MapNodeType encounterType, out float common, out float uncommon, out float rare)
    {
        switch (encounterType)
        {
            case MapNodeType.ELITE_ENEMY:
                common = 50f;
                uncommon = 40f;
                rare = 10f;
                return;
            case MapNodeType.BOSS:
                common = 0f;
                uncommon = 0f;
                rare = 100f;
                return;
            case MapNodeType.MERCHANT:
                common = 54f;
                uncommon = 38f;
                rare = 8f;
                return;
            case MapNodeType.MINOR_ENEMY:
            default:
                common = 60f;
                uncommon = 38f;
                rare = 2f;
                return;
        }
    }

    private static CardData PickCard(DeckBuildCatalog catalog, CardRarity rarity, HashSet<string> usedNames)
    {
        var pool = new List<CardData>();
        foreach (var c in catalog.Cards)
        {
            if (c == null || c.Rarity != rarity)
            {
                continue;
            }

            if (usedNames.Contains(c.name))
            {
                continue;
            }

            pool.Add(c);
        }

        if (pool.Count > 0)
        {
            return pool[Random.Range(0, pool.Count)];
        }

        foreach (var c in catalog.Cards)
        {
            if (c == null)
            {
                continue;
            }

            if (c.Rarity == CardRarity.Basic)
            {
                continue;
            }

            if (!usedNames.Contains(c.name))
            {
                pool.Add(c);
            }
        }

        if (pool.Count > 0)
        {
            return pool[Random.Range(0, pool.Count)];
        }

        foreach (var c in catalog.Cards)
        {
            if (c != null && !usedNames.Contains(c.name))
            {
                return c;
            }
        }

        foreach (var c in catalog.Cards)
        {
            if (c != null)
            {
                return c;
            }
        }

        return null;
    }
}
