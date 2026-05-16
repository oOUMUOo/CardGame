using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Магазин: 6 фиксированных слотов в сцене (MerchantSlot1…6 с CardPileCardItem,
/// BuyButton1…6 под ними). Редкость — таблица MERCHANT из VictoryCardRewardGenerator.
/// </summary>
public class MerchantShopController : MonoBehaviour
{
    public const int SlotCount = 6;

    [Header("Routes")]
    [SerializeField] private MapSceneRoutesData sceneRoutes;

    [Header("Catalog")]
    [SerializeField] private DeckBuildCatalog cardCatalog;

    [Header("Slots — по порядку 1…6")]
    [SerializeField] private CardPileCardItemUI[] merchantCardDisplays = new CardPileCardItemUI[SlotCount];
    [SerializeField] private Button[] buyButtons = new Button[SlotCount];
    [Tooltip("Текст цены для каждого слота; если пусто — ищется TMP на кнопке.")]
    [SerializeField] private TMP_Text[] priceTexts = new TMP_Text[SlotCount];

    [Header("UI")]
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text goldText;

    private readonly CardData[] _offeredCards = new CardData[SlotCount];
    private readonly bool[] _sold = new bool[SlotCount];

    private void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnClickBack);
        }

        BuildOffers();
        RefreshGoldDisplay();
        RefreshBuyButtonsAffordability();
    }

    private void BuildOffers()
    {
        if (cardCatalog == null)
        {
            Debug.LogError("MerchantShopController: назначьте DeckBuildCatalog.");
            return;
        }

        List<CardData> offered = VictoryCardRewardGenerator.GenerateRewardOffers(
            MapNodeType.MERCHANT,
            cardCatalog,
            SlotCount);

        for (var i = 0; i < SlotCount; i++)
        {
            _sold[i] = false;
            CardData card = i < offered.Count ? offered[i] : null;
            _offeredCards[i] = card;

            var display = i < merchantCardDisplays.Length ? merchantCardDisplays[i] : null;
            if (display != null)
            {
                if (card != null)
                {
                    display.gameObject.SetActive(true);
                    display.Setup(card);
                }
                else
                {
                    display.gameObject.SetActive(false);
                }
            }

            int price = card != null ? Mathf.Max(0, card.PurchasePrice) : 0;
            TMP_Text priceLabel = GetPriceText(i);
            if (priceLabel != null)
            {
                priceLabel.text = card != null ? price.ToString() : string.Empty;
            }

            Button btn = i < buyButtons.Length ? buyButtons[i] : null;
            if (btn != null)
            {
                var capturedIndex = i;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnClickBuy(capturedIndex));
                btn.interactable = card != null;
            }
        }
    }

    private TMP_Text GetPriceText(int index)
    {
        if (index >= 0 && index < priceTexts.Length && priceTexts[index] != null)
        {
            return priceTexts[index];
        }

        if (index >= 0 && index < buyButtons.Length && buyButtons[index] != null)
        {
            return buyButtons[index].GetComponentInChildren<TMP_Text>(true);
        }

        return null;
    }

    private void OnClickBuy(int index)
    {
        if (index < 0 || index >= SlotCount || _sold[index])
        {
            return;
        }

        CardData data = _offeredCards[index];
        if (data == null)
        {
            return;
        }

        int price = Mathf.Max(0, data.PurchasePrice);
        if (!RunSaveSystem.TrySpendGold(price))
        {
            return;
        }

        RunDeckBonusCardsSystem.AddCard(data.name);
        _sold[index] = true;

        if (index < buyButtons.Length && buyButtons[index] != null)
        {
            buyButtons[index].interactable = false;
        }

        RefreshGoldDisplay();
        RefreshBuyButtonsAffordability();
    }

    private void RefreshBuyButtonsAffordability()
    {
        int gold = RunSaveSystem.LoadGold();
        for (var i = 0; i < SlotCount; i++)
        {
            if (_sold[i] || _offeredCards[i] == null)
            {
                continue;
            }

            if (i >= buyButtons.Length || buyButtons[i] == null)
            {
                continue;
            }

            int price = Mathf.Max(0, _offeredCards[i].PurchasePrice);
            buyButtons[i].interactable = gold >= price;
        }
    }

    private void RefreshGoldDisplay()
    {
        if (goldText != null)
        {
            goldText.text = RunSaveSystem.LoadGold().ToString();
        }
    }

    private void OnClickBack()
    {
        if (sceneRoutes == null)
        {
            Debug.LogError("MerchantShopController: назначьте MapSceneRoutesData.");
            return;
        }

        MapEncounterBridge.CompleteEncounterAndLoadMap(sceneRoutes);
    }
}
