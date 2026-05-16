using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;
    [SerializeField] private CardPileButtonUI drawPileUI;
    [SerializeField] private CardPileButtonUI discardPileUI;
    [SerializeField] private CardPilePanelUI cardPilePanelUI;

    private readonly List<Card> drawPile = new();
    private readonly List<Card> discardPile = new();
    private readonly List<Card> hand = new();

    void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);      
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
    }

    //Publics

    public void Setup(List<CardData> deckData)
    {
        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();

        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }

        SetupPileUI();
        RefreshPileUI();
    }

    //Performers

    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        int actualAmount = Mathf.Min(drawCardsGA.Amount, drawPile.Count);
        int notDrawnAmount = drawCardsGA.Amount - actualAmount;
        for (int i = 0; i < actualAmount; i++)
        {
            yield return DrawCard();
        }

        if (notDrawnAmount > 0)
        {
            RefillDeck();
            for (int i = 0; i < notDrawnAmount; i++)
            {
                yield return DrawCard();
            }
        }

        RefreshPileUI();
    }

    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        foreach (var card in hand)
        {
            CardView cardView = handView.RemoveCard(card);
            yield return DiscardCard(cardView);
        }

        hand.Clear();
        RefreshPileUI();
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        yield return DiscardCard(cardView);

        SpendManaGA spendManaGA = new(playCardGA.Card.Mana);
        ActionSystem.Instance.AddReaction(spendManaGA);
        
        if (playCardGA.Card.ManualTargetEffect != null)
        {
            PerformEffectGA performEffectGA = new(playCardGA.Card.ManualTargetEffect, new() {playCardGA.ManualTarget});
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
        foreach (var effectWrapper in playCardGA.Card.OtherEffects)
        {
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
            PerformEffectGA performEffectGA = new(effectWrapper.Effect, targets);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }

        RefreshPileUI();
    }  

    //Helpers

    private IEnumerator DrawCard()
    {
        Card card = drawPile.Draw();
        if (card == null)
        {
            yield break;
        }

        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        yield return handView.AddCard(cardView);
        RefreshPileUI();
    }

    private void RefillDeck()
    {
        if (drawPile.Count > 0 || discardPile.Count == 0)
        {
            return;
        }

        drawPile.AddRange(discardPile);
        discardPile.Clear();
        RefreshPileUI();
    }

    private IEnumerator DiscardCard(CardView cardView)
    {
        if (cardView == null || cardView.Card == null)
        {
            yield break;
        }

        discardPile.Add(cardView.Card);
        cardView.transform.DOScale(Vector3.zero, 0.15f);
        Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
        RefreshPileUI();
    }

    private void SetupPileUI()
    {
        if (drawPileUI != null)
        {
            drawPileUI.Setup("Добор", OpenDrawPile);
        }

        if (discardPileUI != null)
        {
            discardPileUI.Setup("Сброс", OpenDiscardPile);
        }
    }

    private void RefreshPileUI()
    {
        if (drawPileUI != null)
        {
            drawPileUI.SetCount(drawPile.Count);
        }

        if (discardPileUI != null)
        {
            discardPileUI.SetCount(discardPile.Count);
        }
    }

    private void OpenDrawPile()
    {
        if (cardPilePanelUI == null)
        {
            return;
        }

        cardPilePanelUI.Show("Стопка добора", drawPile);
    }

    private void OpenDiscardPile()
    {
        if (cardPilePanelUI == null)
        {
            return;
        }

        cardPilePanelUI.Show("Стопка сброса", discardPile);
    }
}
