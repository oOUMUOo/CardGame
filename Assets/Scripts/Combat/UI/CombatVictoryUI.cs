using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatVictoryUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Rewards")]
    [SerializeField] private TMP_Text goldRewardText;
    [SerializeField] private TMP_Text additionalRewardText;
    [SerializeField] private int goldReward = 25;
    [SerializeField] private string additionalReward = "Choose 1 card";

    [Header("Card reward (assign in Inspector)")]
    [SerializeField] private DeckBuildCatalog rewardCardCatalog;
    [SerializeField] private Button cardsButton;
    [SerializeField] private GameObject cardChoicePanel;
    [SerializeField] private Transform cardRewardItemsRoot;
    [SerializeField] private CardPileCardItemUI cardRewardItemPrefab;
    [SerializeField] private Button skipCardChoiceButton;

    [Header("Navigation")]
    [SerializeField] private MapSceneRoutesData sceneRoutes;
    [SerializeField] private Button continueButton;
    private bool blocksInteractions;

    private readonly List<CardData> _rolledRewards = new();
    private readonly List<CardPileCardItemUI> _spawnedItems = new();

    private bool _pickedRewardCard;

    private void Awake()
    {
        if (root == null)
        {
            root = gameObject;
        }

        if (continueButton == null)
        {
            continueButton = GetComponent<Button>();
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnClickContinue);
            continueButton.onClick.AddListener(OnClickContinue);
        }

        if (cardsButton != null)
        {
            cardsButton.onClick.RemoveListener(OnClickCards);
            cardsButton.onClick.AddListener(OnClickCards);
        }

        if (skipCardChoiceButton != null)
        {
            skipCardChoiceButton.onClick.RemoveListener(OnClickSkipCardChoice);
            skipCardChoiceButton.onClick.AddListener(OnClickSkipCardChoice);
        }

        if (cardChoicePanel != null)
        {
            cardChoicePanel.SetActive(false);
        }

        Hide();
    }

    public void Show()
    {
        if (root != null)
        {
            root.SetActive(true);
            root.transform.localScale = Vector3.one;
        }

        if (!blocksInteractions && Interactions.Instance != null)
        {
            Interactions.Instance.PushUIBlocker();
            blocksInteractions = true;
        }

        if (goldRewardText != null)
        {
            goldRewardText.text = $"+{goldReward} Золото";
        }

        if (additionalRewardText != null)
        {
            additionalRewardText.text = additionalReward;
        }

        _pickedRewardCard = false;
        RegenerateRewardCardsAndUi();
        UpdateCardsButtonInteractable();

        if (cardChoicePanel != null)
        {
            cardChoicePanel.SetActive(false);
        }
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

    public void OnClickContinue()
    {
        RunSaveSystem.AddGold(goldReward);

        if (HeroSystem.Instance != null)
        {
            HeroSystem.Instance.SaveCurrentHealth();
        }

        if (sceneRoutes == null)
        {
            Debug.LogError("CombatVictoryUI: MapSceneRoutesData is not assigned.");
            return;
        }

        MapEncounterBridge.CompleteEncounterAndLoadMap(sceneRoutes);
    }

    private void OnClickCards()
    {
        if (_rolledRewards.Count == 0)
        {
            return;
        }

        if (cardChoicePanel != null)
        {
            cardChoicePanel.SetActive(true);
        }
    }

    private void OnClickSkipCardChoice()
    {
        if (cardChoicePanel != null)
        {
            cardChoicePanel.SetActive(false);
        }
    }

    private void OnClickPickCard(int index)
    {
        if (_pickedRewardCard || index < 0 || index >= _rolledRewards.Count)
        {
            return;
        }

        var data = _rolledRewards[index];
        if (data != null)
        {
            RunDeckBonusCardsSystem.AddCard(data.name);
        }

        _pickedRewardCard = true;
        foreach (var item in _spawnedItems)
        {
            if (item != null)
            {
                item.SetInteractable(false);
            }
        }

        if (cardChoicePanel != null)
        {
            cardChoicePanel.SetActive(false);
        }

        UpdateCardsButtonInteractable();
    }

    private void UpdateCardsButtonInteractable()
    {
        if (cardsButton == null)
        {
            return;
        }

        cardsButton.interactable = _rolledRewards.Count > 0 && !_pickedRewardCard;
    }

    private void RegenerateRewardCardsAndUi()
    {
        ClearRewardItems();
        _rolledRewards.Clear();

        if (rewardCardCatalog == null || cardRewardItemPrefab == null || cardRewardItemsRoot == null)
        {
            return;
        }

        MapNodeType nodeType = MapNodeType.MINOR_ENEMY;
        if (MapSaveSystem.TryLoad(out MapSaveData save) && save.PendingEncounterLayer >= 0 && save.PendingEncounterIndex >= 0)
        {
            nodeType = save.PendingNodeType;
        }

        _rolledRewards.AddRange(VictoryCardRewardGenerator.GenerateThreeRewards(nodeType, rewardCardCatalog));

        foreach (var card in _rolledRewards)
        {
            CardPileCardItemUI item = Instantiate(cardRewardItemPrefab, cardRewardItemsRoot);
            item.Setup(card);
            var idx = _spawnedItems.Count;
            item.SetClick(() => OnClickPickCard(idx));
            _spawnedItems.Add(item);
        }
    }

    private void ClearRewardItems()
    {
        foreach (var item in _spawnedItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        _spawnedItems.Clear();
    }
}
