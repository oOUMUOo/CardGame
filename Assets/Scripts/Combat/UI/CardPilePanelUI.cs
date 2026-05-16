using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPilePanelUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Transform listContentRoot;
    [SerializeField] private CardPileCardItemUI cardItemPrefab;

    private readonly List<CardPileCardItemUI> spawnedItems = new();
    private bool blocksInteractions;

    private void Awake()
    {
        Hide();
    }

    public void Show(string panelTitle, IReadOnlyList<Card> cards)
    {
        if (root != null)
        {
            root.SetActive(true);
        }

        if (!blocksInteractions && Interactions.Instance != null)
        {
            Interactions.Instance.PushUIBlocker();
            blocksInteractions = true;
        }

        if (titleText != null)
        {
            titleText.text = panelTitle;
        }

        RebuildList(cards);
    }

    public void Hide()
    {
        if (blocksInteractions && Interactions.Instance != null)
        {
            Interactions.Instance.PopUIBlocker();
            blocksInteractions = false;
        }

        if (root != null)
        {
            root.SetActive(false);
        }
    }

    public void OnClickClose()
    {
        Hide();
    }

    private void RebuildList(IReadOnlyList<Card> cards)
    {
        ClearItems();

        if (cards == null || cardItemPrefab == null || listContentRoot == null)
        {
            return;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            CardPileCardItemUI item = Instantiate(cardItemPrefab, listContentRoot);
            item.Setup(card);
            spawnedItems.Add(item);
        }
    }

    private void ClearItems()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        spawnedItems.Clear();
    }
}
