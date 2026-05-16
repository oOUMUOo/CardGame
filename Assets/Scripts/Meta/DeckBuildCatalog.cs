using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Deck Build Catalog", fileName = "DeckBuildCatalog")]
public class DeckBuildCatalog : ScriptableObject
{
    [SerializeField] private List<CardData> cards = new();

    public IReadOnlyList<CardData> Cards => cards;

    public bool TryGetByAssetName(string assetName, out CardData data)
    {
        foreach (var c in cards)
        {
            if (c != null && c.name == assetName)
            {
                data = c;
                return true;
            }
        }

        data = null;
        return false;
    }
}
