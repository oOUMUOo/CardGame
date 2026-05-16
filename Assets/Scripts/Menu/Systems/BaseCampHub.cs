using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseCampHub : MonoBehaviour
{
    private const int DeckSize = DeckPresetSaveSystem.RequiredDeckSize;

    [Header("Data")]
    [SerializeField] private MapSceneRoutesData sceneRoutes;
    [SerializeField] private DeckBuildCatalog deckCatalog;

    [Header("Deck builder — префаб как в CardPilePanel")]
    [SerializeField] private CardPileCardItemUI cardPileItemPrefab;
    [Tooltip("Масштаб экземпляров в вертикальном списке доступных карт.")]
    [SerializeField] private float poolCardsScale = 0.35f;
    [Tooltip("Масштаб карточек в горизонтальном PickedCardsStrip (Scroll View → Content).")]
    [SerializeField] private float pickedCardsScale = 0.22f;

    [Header("Canvas — главная полоса")]
    [SerializeField] private Button configureDeckButton;
    [SerializeField] private Button startRunButton;

    [Header("Canvas — панель колоды")]
    [SerializeField] private GameObject deckBuilderPanel;
    [SerializeField] private Button closeDeckBuilderButton;
    [SerializeField] private TMP_Text deckCounterText;
    [SerializeField] private Button saveDeckButton;
    [Tooltip("Content горизонтального ScrollView выбранных карт (Horizontal Layout Group).")]
    [SerializeField] private RectTransform pickedCardsContent;
    [Tooltip("Content вертикального ScrollView списка разблокированных карт.")]
    [SerializeField] private RectTransform cardListContent;

    private readonly List<CardData> _picked = new();

    private void Awake()
    {
        if (!ValidateManualUi())
        {
            enabled = false;
            return;
        }

        configureDeckButton.onClick.AddListener(OpenDeckBuilder);
        startRunButton.onClick.AddListener(OnStartRun);
        saveDeckButton.onClick.AddListener(OnSaveDeck);
        if (closeDeckBuilderButton != null)
        {
            closeDeckBuilderButton.onClick.AddListener(CloseDeckBuilder);
        }

        if (deckBuilderPanel != null && deckBuilderPanel.activeSelf)
        {
            deckBuilderPanel.SetActive(false);
        }
    }

    private bool ValidateManualUi()
    {
        var ok = true;
        if (configureDeckButton == null)
        {
            Debug.LogError("BaseCampHub: назначьте Configure Deck Button.");
            ok = false;
        }

        if (startRunButton == null)
        {
            Debug.LogError("BaseCampHub: назначьте Start Run Button.");
            ok = false;
        }

        if (deckBuilderPanel == null)
        {
            Debug.LogError("BaseCampHub: назначьте Deck Builder Panel.");
            ok = false;
        }

        if (deckCounterText == null)
        {
            Debug.LogError("BaseCampHub: назначьте Deck Counter Text.");
            ok = false;
        }

        if (saveDeckButton == null)
        {
            Debug.LogError("BaseCampHub: назначьте Save Deck Button.");
            ok = false;
        }

        if (pickedCardsContent == null)
        {
            Debug.LogError("BaseCampHub: назначьте Picked Cards Content (RectTransform Content горизонтального Scroll View).");
            ok = false;
        }

        if (cardListContent == null)
        {
            Debug.LogError("BaseCampHub: назначьте Card List Content.");
            ok = false;
        }

        if (cardPileItemPrefab == null)
        {
            Debug.LogError("BaseCampHub: назначьте Card Pile Item Prefab (CardPileCardItem).");
            ok = false;
        }

        return ok;
    }

    private void CloseDeckBuilder()
    {
        if (deckBuilderPanel != null)
        {
            deckBuilderPanel.SetActive(false);
        }
    }

    private void OpenDeckBuilder()
    {
        if (deckCatalog == null)
        {
            Debug.LogError("BaseCampHub: назначьте DeckBuildCatalog.");
            return;
        }

        CardUnlockSaveSystem.EnsureSeededFromCatalog(deckCatalog);
        _picked.Clear();
        if (DeckPresetSaveSystem.TryLoad(out var names) && names.Length == DeckSize)
        {
            foreach (var n in names)
            {
                if (deckCatalog.TryGetByAssetName(n, out var c))
                {
                    _picked.Add(c);
                }
            }

            if (_picked.Count != DeckSize)
            {
                _picked.Clear();
            }
        }

        deckBuilderPanel.SetActive(true);
        RebuildCardList();
        RebuildPickedStrip();
        RefreshCounter();
    }

    private void RebuildCardList()
    {
        ClearChildren(cardListContent);

        var pool = CardUnlockSaveSystem.GetUnlockedCardsSorted(deckCatalog);
        foreach (var cardData in pool)
        {
            var item = Instantiate(cardPileItemPrefab, cardListContent);
            item.Setup(cardData);
            ApplyItemScale(item.transform, poolCardsScale);
            WireItemButton(item, () => OnClickPoolCard(cardData));
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(cardListContent);
    }

    private void OnClickPoolCard(CardData cardData)
    {
        if (_picked.Count >= DeckSize || cardData == null)
        {
            return;
        }

        _picked.Add(cardData);
        RebuildPickedStrip();
        RefreshCounter();
    }

    private void RebuildPickedStrip()
    {
        ClearChildren(pickedCardsContent);

        for (var i = 0; i < _picked.Count; i++)
        {
            var idx = i;
            var cardData = _picked[i];
            var item = Instantiate(cardPileItemPrefab, pickedCardsContent);
            item.Setup(cardData);
            ApplyItemScale(item.transform, pickedCardsScale);
            WireItemButton(item, () => OnClickPickedCard(idx));
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(pickedCardsContent);
    }

    private void OnClickPickedCard(int index)
    {
        if (index < 0 || index >= _picked.Count)
        {
            return;
        }

        _picked.RemoveAt(index);
        RebuildPickedStrip();
        RefreshCounter();
    }

    private static void ClearChildren(RectTransform parent)
    {
        if (parent == null)
        {
            return;
        }

        for (var c = parent.childCount - 1; c >= 0; c--)
        {
            Destroy(parent.GetChild(c).gameObject);
        }
    }

    private static void ApplyItemScale(Transform t, float uniformScale)
    {
        if (t == null || uniformScale <= 0f)
        {
            return;
        }

        t.localScale = new Vector3(uniformScale, uniformScale, 1f);
    }

    private static void WireItemButton(CardPileCardItemUI item, UnityEngine.Events.UnityAction onClick)
    {
        if (item == null || onClick == null)
        {
            return;
        }

        var btn = item.GetComponent<Button>();
        if (btn == null)
        {
            btn = item.gameObject.AddComponent<Button>();
            btn.targetGraphic = item.GetComponentInChildren<Image>(true);
            btn.transition = Selectable.Transition.None;
        }

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClick);
    }

    private void RefreshCounter()
    {
        if (deckCounterText != null)
        {
            deckCounterText.text = $"{_picked.Count} / {DeckSize}";
        }

        if (saveDeckButton != null)
        {
            saveDeckButton.interactable = _picked.Count == DeckSize;
        }
    }

    private void OnSaveDeck()
    {
        if (_picked.Count != DeckSize)
        {
            return;
        }

        var names = new string[DeckSize];
        for (var i = 0; i < DeckSize; i++)
        {
            names[i] = _picked[i].name;
        }

        DeckPresetSaveSystem.Save(names);
        CloseDeckBuilder();
    }

    private void OnStartRun()
    {
        if (sceneRoutes == null)
        {
            Debug.LogError("BaseCampHub: назначьте MapSceneRoutesData.");
            return;
        }

        SceneManager.LoadScene(sceneRoutes.MapSceneName);
    }
}
